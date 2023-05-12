using System.Collections.Generic;
using MonoGame.Extended.Entities;
using Underall.Helpers;
using Underall.MetaInfo;

namespace Underall.World;

public class World
{
    internal WorldGrid Grid;
    private MonoGame.Extended.Entities.World _World;
    private LevelInfo CurrentLevel;

    public World(SaveInfo saveInfo)
    {
        InitializeSystems();
        CurrentLevel = saveInfo.CurrentLevel;
        Grid = new WorldGrid(
            CurrentLevel.GridWidth, 
            CurrentLevel.GridHeight, 
            CurrentLevel.GridCellWidth,
            CurrentLevel.GridCellHeight
            );
    }

    public void InitializeSystems()
    {
        _World = new WorldBuilder()
            // add systems here by AddSystem method 
            .Build();
    }
}