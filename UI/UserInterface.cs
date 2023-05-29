using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Underall.Components;
using Underall.Helpers;
using Underall.MetaInfo;
using Underall.UI;
using Underall.UI.Bars;
using Underall.UI.Buttons;
using Underall.UI.Frames;

using MenuButton = Underall.UI.Buttons.RelativeButton;

namespace Underall.Systems;

public class UserInterface
{
    public Game1 Game { get; private set; }
    private SpriteBatch SpriteBatch { get; set; }
    private ContentManager ContentManager { get; set; }
    public readonly Vector2 ScreenDimensions = new(400, 300);
    
    #region GameplayStuff
    private int ControllableEntityId { get; set; }
    private CBasicStats ControllableBasicStats { get; set; }
    private CSizePosition ControllableSizePosition { get; set;}
    public StatBar HealthBar { get; set; }
    public StatBar StaminaBar { get; set; }
    private StatBar SanityBar { get; set; }
    private RelativeFrame DialogueFrame { get; set; }
    #endregion
    
    #region MainMenuStuff
    public List<MenuButton> MainMenuButtons { get; private set; }
    #endregion
    
    #region PauseMenuStuff
    public List<MenuButton> PauseMenuButtons { get; private set; }
    #endregion
    
    #region SaveMenuStuff
    private static readonly int MaxSaveCount = SaveHelper.MaxSaveCount;
    private static readonly float SaveLongButtonXPosition = 0.05f;
    private static Vector2 LoadMenuTextScale { get; set; }
    #endregion

    public UserInterface(Game1 game, SpriteBatch spriteBatch, ContentManager contentManager, ConfigInfo config)
    {
        Game = game;
        SpriteBatch = spriteBatch;
        ContentManager = contentManager;
        ScreenDimensions = new Vector2(spriteBatch.GraphicsDevice.Viewport.Width,
            spriteBatch.GraphicsDevice.Viewport.Height);
        InitializeLoadMenuUI(config);
        InitializeFrames(config);
    }

    public void InitializeMainMenuUI(ConfigInfo config)
    {
        MainMenuButtons = new List<RelativeButton>();
        MainMenuButtons.Add(GetInitializedButton(config.MainMenu_StartGameButton));
        MainMenuButtons.Add(GetInitializedButton(config.MainMenu_LoadSaveButton));
        MainMenuButtons.Add(GetInitializedButton(config.MainMenu_ExitButton));
    }

    public void InitializePauseMenuUI(ConfigInfo config)
    {
        PauseMenuButtons = new List<RelativeButton>();
        PauseMenuButtons.Add(GetInitializedButton(config.PauseMenu_ReturnButton));
        PauseMenuButtons.Add(GetInitializedButton(config.PauseMenu_MakeSaveButton));
        PauseMenuButtons.Add(GetInitializedButton(config.PauseMenu_LoadSaveButton));
        PauseMenuButtons.Add(GetInitializedButton(config.PauseMenu_ExitButton));
    }

    private MenuButton GetInitializedButton(MenuButton button) =>
        button.Initialized(this, SpriteBatch, ContentManager, ScreenDimensions);

    public void InitializeGameplayUI(ConfigInfo config)
    {
        ControllableEntityId = Game.World.ControllableEntityId;
        FetchControllableComponents(Game.World.ComponentManager);
        InitializeGameplayStatBars(config);
    }

    /// <summary>
    /// This method should be called inside UserInterface constructor, because initialization of other LoadMenuStuff,
    /// such as buttons and text, happens during loading the screen.
    /// </summary>
    private void InitializeLoadMenuUI(ConfigInfo config)
    {
        LoadMenuTextScale = config.LoadMenuTextScale;
    }

    /// <summary>
    /// Returns raw save buttons with DummyFunction that should be replaced with another one
    /// </summary>
    private IEnumerable<MenuButton> GetSaveButtons(ConfigInfo config)
    {
        return SaveHelper.GetSaveNames()
            .Zip(Enumerable.Range(0, SaveHelper.MaxSaveCount))
            .Select(save => GetInitializedButton(CloneHelper.Clone(config.LoadMenu_LongButton))
                .WithUVPosition(new Vector2(SaveLongButtonXPosition, 0.2f + save.Second * 0.15f), ScreenDimensions)
                .WithNewTextItem(save.First, config.StandardRelativeFrameTextItem, LoadMenuTextScale)
                .Initialized(this, SpriteBatch, ContentManager, ScreenDimensions)
            );
    }

    public IEnumerable<MenuButton> GetSaveButtonsToMakeSave(ConfigInfo config) => 
        GetMenuButtonsWithAssignedFunction(GetSaveButtons(config), config, "MakeSave");

    public IEnumerable<MenuButton> GetSaveButtonsToLoadSave(ConfigInfo config) =>
        GetMenuButtonsWithAssignedFunction(GetSaveButtons(config), config, "LoadSave");
    

    private IEnumerable<MenuButton> GetMenuButtonsWithAssignedFunction(IEnumerable<MenuButton> buttons, 
        ConfigInfo config, string functionName) => 
        buttons.Select(button => button.WithOnClickFunction(UIFunctionRegistry.Functions[functionName]));

    public void DrawGameplay()
    {
        SpriteBatch.Begin(SpriteSortMode.BackToFront);
        HealthBar.Draw();
        StaminaBar.Draw();
        // SanityBar.Draw();
        
        SpriteBatch.End();
    }

    private void FetchControllableComponents(ComponentManager componentManager)
    {
        ControllableSizePosition = componentManager.GetMapper<CSizePosition>().Get(ControllableEntityId);
        ControllableBasicStats = componentManager.GetMapper<CBasicStats>().Get(ControllableEntityId);
    }

    private void InitializeGameplayStatBars(ConfigInfo config)
    {
        HealthBar = config.HealthBarBuilder
            .AddStatInfo(ControllableBasicStats.Health)
            .AddSpriteBatch(SpriteBatch)
            .BindSource(ContentManager)
            .Build();
        SanityBar = config.SanityBarBuilder
            .AddStatInfo(ControllableBasicStats.Sanity)
            .AddSpriteBatch(SpriteBatch)
            .BindSource(ContentManager)
            .Build();
        StaminaBar = config.StaminaBarBuilder
            .AddStatInfo(ControllableBasicStats.Stamina)
            .AddSpriteBatch(SpriteBatch)
            .BindSource(ContentManager)
            .Build();
    }

    private void InitializeFrames(ConfigInfo config)
    {
        DialogueFrame = config.DialogueFrame;
        DialogueFrame.Initialize(this, SpriteBatch, ContentManager, ScreenDimensions);
    }
}