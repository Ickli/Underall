using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using Underall.Components;
using Underall.Helpers;
using Underall.MetaInfo;
using Underall.Systems;

namespace Underall.World;

public class World
{
    public WorldGrid Grid { get; private set; }
    public MonoGame.Extended.Entities.World _World { get; private set; }
    public LevelInfo CurrentLevel { get; private set; }
    public ComponentManager ComponentManager { get; private set; }
    public Dimensions Dimension { get; private set; }

    public int ControllableEntityId = 0; // TODO: CHANGE THIS
    
    public World(Game1 game, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, string saveFolderName, ConfigInfo config)
    {
        
        LoadLevelInfoFromFolder(saveFolderName);
        InitializeSystems(game, graphicsDevice, spriteBatch, config);
        // game.Components.Add(_World);
        InitializeComponentManager();
        LoadScreenInfo(saveFolderName);
        BoundTexturesToSpriteComponents(game.Content);

        Grid = GetGridPopulatedWithCurrentEntities(config);
        game.World = this;
    }

    /// <summary>
    /// All systems, that are meant to be used, have to be added here
    /// </summary>
    private void InitializeSystems(Game1 game, 
        GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, ConfigInfo config)
    {
        _World = new WorldBuilder()
             .AddSystem(new SControls(game, this, config))
             .AddSystem(new SPhysics(this, config))
             .AddSystem(new SDraw(this, graphicsDevice, spriteBatch, game.Content, CurrentLevel))
             .Build();
    }

    // public void Update()
    // {
    //     ChangeGridIfControllableOutOfScreen(ControllablePos);
    // }

    public void ChangeGridIfControllableOutOfScreen(CSizePosition controllablePosition)
    {
        if(!Grid.Contains(controllablePosition))
            Grid.Populate(controllablePosition, ComponentManager.GetMapper<CSizePosition>().Components);
    }

    private void LoadLevelInfoFromFolder(string saveFolderName) =>
        CurrentLevel = SaveHelper.GetLevelInfo(saveFolderName);

    private void LoadScreenInfo(string saveFolderName) => 
        SaveHelper.LoadSavedScreen(_World, PathHelper.GetFullPathToScreenInfo(saveFolderName, CurrentLevel));

    private WorldGrid GetGridPopulatedWithCurrentEntities(ConfigInfo config)
    {
        var components = ComponentManager.GetMapper<CSizePosition>().Components;
        return new WorldGrid(config, CurrentLevel, components[ControllableEntityId], components);
    }
    

    public void BoundTexturesToSpriteComponents(ContentManager contentManager) =>
        ComponentRegistry.BindComponentSources(ComponentManager, contentManager);
    

    private void InitializeComponentManager()
    {
        ComponentManager = 
            ReflectionHelper.GetPrivateProperty<MonoGame.Extended.Entities.World, ComponentManager>(_World, "ComponentManager");
        ComponentRegistry.InitializeComponentMappers(ComponentManager);
    }

    public void MakeSave(string saveFolderName)
    {
        SaveHelper.MakeSave(_World, CurrentLevel, saveFolderName);
    }
}