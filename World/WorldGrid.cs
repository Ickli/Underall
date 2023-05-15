using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using Underall.Components;
using Underall.MetaInfo;

namespace Underall.World;

/// <summary>
/// The class interprets screen as a grid, so it allows to have cheap collision detection and low memory usage
/// by containing only part of a level.
/// </summary>
internal class WorldGrid
{
    // Key/value pairs where key is depth of the entity
    public SortedList<int, int>[,] Grid { get; private set; }
    public Vector2 TopLeftCorner;
    public int Width => Grid.GetLength(0);
    public int Height => Grid.GetLength(1);
    public int CellWidth { get; private set; }
    public int CellHeight { get; private set; }
    
    public WorldGrid(int width, int height, int cellWidth, int cellHeight, Bag<CSizePosition> cSizePositions = null)
    {
        InitializeGrid(width, height);
        CellHeight = cellHeight;
        CellWidth = cellWidth;
        
        if(cSizePositions is not null)
            Populate(cSizePositions);
    }

    public WorldGrid(LevelInfo levelInfo, Bag<CSizePosition> cSizePositions = null) 
        : this(levelInfo.GridWidth, 
            levelInfo.GridHeight, 
            levelInfo.GridCellWidth, 
            levelInfo.GridCellHeight,
            cSizePositions) {}

    /// <summary>
    /// Populates the world by fetching entities in area from X Y position to X+Width*CellWidth Y+Height*CellHeight
    /// </summary>
    public void Populate(Bag<CSizePosition> cSizePositions)
    {
        for(var entityId = 0; entityId < cSizePositions.Count; entityId++)
            GetCellByPixelPosition(cSizePositions[entityId].Location).Add(cSizePositions[entityId].Depth, entityId);
    }

    private SortedList<int, int> GetCellByPixelPosition(Vector2 position)
    {
        var gridCoordinates = (position - TopLeftCorner);
        gridCoordinates.X /= CellWidth;
        gridCoordinates.Y /= CellHeight;
        return Grid[(int)gridCoordinates.X, (int)gridCoordinates.Y];
    }

    public SortedList<int, int> this[int width, int height]
    {
        get => Grid[width, height];
    }

    private void InitializeGrid(int width, int height)
    {
        Grid = new SortedList<int, int>[width, height];
        for(var i = width - 1; i >= 0; i--)
        for (var j = height - 1; j >= 0; j--)
            Grid[i, j] = new SortedList<int, int>();
    }

    private void Clear()
    {
        for(var i = Width - 1; i >= 0; i--)
        for (var j = Height - 1; j >= 0; j--)
            Grid[i, j].Clear();
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