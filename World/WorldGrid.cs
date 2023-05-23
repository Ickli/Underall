using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using Underall.Collections;
using Underall.Components;
using Underall.MetaInfo;

namespace Underall.World;

using GridCell = SortedLinkedList<CSizePosition, int>;

/// <summary>
/// The class interprets screen as a grid, so it allows to have cheap collision detection and low memory usage
/// by containing only part of a level.
/// </summary>
public class WorldGrid
{
    // Key/value pairs where key is depth of the entity
    public GridCell[,] Grid { get; private set; }
    public List<HashSet<int>> LayeredGrid { get; private set; }
    public Vector2 TopLeftCorner;
    public int Width => Grid.GetLength(0);
    public int Height => Grid.GetLength(1);

    public int CellWidth
    {
        get => _cellWidth;
        private set
        {
            _cellWidth = value;
            UpdatePixelDimensions();
        }
    }

    public int CellHeight
    {
        get => _cellHeight;
        private set
        {
            _cellHeight = value;
            UpdatePixelDimensions();
        }
    }

    private CSizePosition _screenSizePosition;
    private bool _isSizePositionChanged = true;
    private int _cellWidth;
    private int _cellHeight;
    private int _pixelWidth;
    private int _pixelHeight;

    private WorldGrid(ConfigInfo config, int width, int height, int cellWidth, int cellHeight, CSizePosition controllablePosition, 
        Bag<CSizePosition> cSizePositions = null)
    {
        InitializeGrid(width, height);
        InitializeLayeredGrid(config.DefaultLayeredGridDepth);
        _cellHeight = cellHeight;
        _cellWidth = cellWidth;
        UpdatePixelDimensions();
        
        if(cSizePositions is not null)
            Populate(controllablePosition, cSizePositions);
    }

    public WorldGrid(ConfigInfo configInfo, LevelInfo levelInfo, CSizePosition controllablePosition, Bag<CSizePosition> cSizePositions = null) 
        : this(configInfo,
            levelInfo.GridWidths[levelInfo.CurrentLevelId], 
            levelInfo.GridHeights[levelInfo.CurrentLevelId], 
            levelInfo.GridCellWidths[levelInfo.CurrentLevelId], 
            levelInfo.GridCellHeights[levelInfo.CurrentLevelId],
            controllablePosition,
            cSizePositions) {}

    /// <summary>
    /// Populates the grid by fetching entities that are nearby the controllable
    /// </summary>
    public void Populate(CSizePosition controllablePosition,Bag<CSizePosition> cSizePositions)
    {
        SwitchToGridContainingControllable(controllablePosition, cSizePositions);
    }

    private void SwitchToGridContainingControllable(CSizePosition controllablePosition, Bag<CSizePosition> cSizePositions)
    {
        Clear();
        
        TopLeftCorner.X = (int)Math.Floor(controllablePosition.Location.X / _pixelWidth) * _pixelWidth;
        TopLeftCorner.Y = (int)Math.Floor(controllablePosition.Location.Y / _pixelHeight) * _pixelHeight;
        
        Console.WriteLine(TopLeftCorner);
        _screenSizePosition = new CSizePosition { Location = TopLeftCorner, Width = Width*CellWidth, Height = Height*CellHeight};
        
        for(var entityId = 0; entityId < cSizePositions.Count; entityId++)
            if (cSizePositions[entityId] is CSizePosition cSizePosition)
                TryAddEntity(cSizePosition, entityId);

        _isSizePositionChanged = false;
    }

    private void GetCellPosByPixelPosition(ref Vector2 position, out int x, out int y)
    {
        x = (int)((position.X - TopLeftCorner.X) / CellWidth);
        y = (int)((position.Y - TopLeftCorner.Y) / CellHeight);
    }

    private void GetCellPosByPixelPosition(int x, int y, out int cellX, out int cellY)
    {
        cellX = (int)((x - TopLeftCorner.X) / CellWidth);
        cellY = (int)((y - TopLeftCorner.Y) / CellHeight);
    }
    
    public GridCell GetCellByPixelPosition(ref Vector2 position)
    {
        GetCellPosByPixelPosition(ref position, out var x, out var y);
        return Grid[x, y];
    }

    /// <summary>
    /// Performs passed action on every cell possibly occupied by cSizePosition
    /// even if the component is not in the grid
    /// </summary>
    /// <param name="cSizePosition">Component of an entity</param>
    /// <param name="action">Takes x and y indices of a cell possibly occupied by component</param>
    private void ChangeCellsWhere(CSizePosition cSizePosition, Action<GridCell> action)
    {
        var cellsOccupied = GetCellsWhere(cSizePosition);
        for (var cellIndex = 0; cellIndex < cellsOccupied.Count; cellIndex++)
            action(cellsOccupied[cellIndex]);
    }


    /// <returns>Return cells possibly occupied by cSizePosition even if they are not</returns>
    public List<GridCell> GetCellsWhere(CSizePosition cSizePosition)
    {
        var corners = cSizePosition.GetCorners(); // top-left, top-right, bottom-right, bottom-left
        GetCellPosByPixelPosition(corners[0].X, corners[0].Y, out var topLeftX, out var topLeftY);
        GetCellPosByPixelPosition(corners[2].X, corners[2].Y, out var bottomRightX, out var bottomRightY);
        var cells = new List<GridCell>();

        topLeftX = Math.Min(Math.Max(0, topLeftX), Width - 1);
        topLeftY = Math.Min(Math.Max(0, topLeftY), Height - 1);
        bottomRightX = Math.Min(Math.Max(0, bottomRightX), Width - 1);
        bottomRightY = Math.Min(Math.Max(0, bottomRightY), Height - 1);
        
        for(; topLeftX <= bottomRightX; topLeftX++)
        for (var y = topLeftY; y <= bottomRightY; y++)
            cells.Add(Grid[topLeftX, y]);
        return cells;
    }

    public bool Contains(CSizePosition cSizePosition) => cSizePosition.Intersects(_screenSizePosition);
    

    private void UpdateScreenSizePosition() =>
        _screenSizePosition = new CSizePosition{Location = TopLeftCorner, Width = Width * CellWidth, Height = Height * CellHeight};

    private void UpdatePixelDimensions()
    {
        _pixelWidth = Width * CellWidth;
        _pixelHeight = Height * CellHeight;
    }
    

    public GridCell this[int width, int height]
    {
        get => Grid[width, height];
    }

    private void InitializeGrid(int width, int height)
    {
        Grid = new GridCell[width, height];
        for(var i = width - 1; i >= 0; i--)
        for (var j = height - 1; j >= 0; j--)
            Grid[i, j] = new GridCell();
    }

    private void InitializeLayeredGrid(int depth)
    {
        LayeredGrid = new List<HashSet<int>>();
        ExpandLayeredGridIfSmaller(depth);
    }

    private void ExpandLayeredGridIfSmaller(int depthLimit)
    {
        for(var d = LayeredGrid.Count; d < depthLimit; d++)
            LayeredGrid.Add(new HashSet<int>());
    }

    public void RemoveEntity(CSizePosition cSizePosition, int entityId)
    {
        ChangeCellsWhere(cSizePosition, cell => cell.RemoveFirst(entityId));
        LayeredGrid[cSizePosition.Depth].Remove(entityId);
    }

    public bool TryAddEntity(CSizePosition cSizePosition, int entityId)
    {
        if (!cSizePosition.Intersects(_screenSizePosition)) return false;
        ChangeCellsWhere(cSizePosition,
            (cell) =>
            {
                cell.Add(cSizePosition, entityId);
                ExpandLayeredGridIfSmaller(cSizePosition.Depth + 1);
                LayeredGrid[cSizePosition.Depth].Add(entityId);
            });
        return true;
    }
    
    private void Clear()
    {
        // clearing Grid
        for(var i = Width - 1; i >= 0; i--)
        for (var j = Height - 1; j >= 0; j--)
            Grid[i, j].Clear();
        
        // clearing LayeredGrid
        for (var d = 0; d < LayeredGrid.Count; d++)
            LayeredGrid[d].Clear();
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not WorldGrid grid) return false;
        return Width == grid.Width 
               && Height == grid.Height 
               && CellWidth == grid.CellWidth 
               && Height == grid.CellHeight;
    }
}