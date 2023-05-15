using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using Underall.Components;
using Underall.Helpers;
using Underall.MetaInfo;
using Underall.Systems;

namespace Underall.World;

public class World
{
    private WorldGrid Grid;
    public MonoGame.Extended.Entities.World _World;
    private LevelInfo CurrentLevel;
    public ComponentManager ComponentManager { get; set; }
    
    public World(Game1 game, string saveFolderName, ConfigInfo config)
    {
        /*
         * 1. Systems
         * 2. Components
         * 3. LevelInfo and SaveInfo
         * 4. Bound textures
         * 5. Populate grid
         */
        _World = GetWorldWithSystemsInitialized(game, config); 
        game.Components.Add(_World);
        InitializeComponentManager();
        LoadSaveFromFolder(saveFolderName);
        BoundTexturesToSpriteComponents(game.Content);
        Grid = GetGridPopulatedWithCurrentEntities();
    }

    /// <summary>
    /// All systems, that are meant to be used, have to be added here
    /// </summary>
    private static MonoGame.Extended.Entities.World GetWorldWithSystemsInitialized(Game1 game, ConfigInfo config)
    {
         return new WorldBuilder()
             .AddSystem(new SPhysics(config))
             .AddSystem(new SControls(game, config))
             .Build();
    }
    
    private void LoadSaveFromFolder(string saveFolderName)
    {
        CurrentLevel = SaveHelper.GetLevelInfo(saveFolderName);
        SaveHelper.LoadSaveInfoWithDefaultOptions(_World, PathHelper.GetFullPathToSaveInfo(saveFolderName));
        
    }

    private WorldGrid GetGridPopulatedWithCurrentEntities() =>
        new WorldGrid(CurrentLevel, ComponentManager.GetMapper<CSizePosition>().Components);
    

    public void BoundTexturesToSpriteComponents(ContentManager contentManager) =>
        ComponentRegistry.BoundTexturesToSpriteComponents(ComponentManager, contentManager);
    

    private void InitializeComponentManager()
    {
        ComponentManager = 
            ReflectionHelper.GetPrivateProperty<MonoGame.Extended.Entities.World, ComponentManager>(_World, "ComponentManager");
        ComponentRegistry.InitializeComponentMappers(ComponentManager);
    }
}