using System.Collections.Generic;
using MonoGame.Extended.Entities;

namespace Underall.World;

/// <summary>
/// The class interprets a screen as a grid, so it allows to have cheap collision detection and low memory usage
/// by containing only part of a level.
/// </summary>
internal class WorldGrid
{
    // Key/value pairs where key is depth of the entity
    public SortedList<int, Entity>[,] Grid { get; private set; }
    public int Width => Grid.GetLength(0);
    public int Height => Grid.GetLength(1);
    public int CellWidth { get; private set; }
    public int CellHeight { get; private set; }

    public WorldGrid(int width, int height, int cellWidth, int cellHeight)
    {
        InitializeGrid(width, height);
        CellHeight = cellHeight;
        CellWidth = cellWidth;
    }
    
    // Populates the world by fetching entities in area from X Y position to X+Width*CellWidth Y+Height*CellHeight
    // public Populate()
    
    public SortedList<int, Entity> this[int width, int height]
    {
        get => Grid[width, height];
    }

    private void InitializeGrid(int width, int height)
    {
        Grid = new SortedList<int, Entity>[width, height];
        for(var i = width - 1; i >= 0; i--)
        for (var j = height - 1; j >= 0; j--)
            Grid[i, j] = new SortedList<int, Entity>();
    }

    private void Clear()
    {
        for(var i = Width - 1; i >= 0; i--)
        for (var j = Height - 1; j >= 0; j--)
            Grid[i, j].Clear();
    }
}