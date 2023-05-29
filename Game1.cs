using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using Underall.Helpers;
using Underall.MetaInfo;
using Underall.Screens;
using Underall.Systems;

namespace Underall;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private ScreenManager _screenManager;
    private SpriteBatch _spriteBatch;
    public ConfigInfo Config { get; private set; }
    private string CurrentSave;

    public int timeDif = 5;

    public World.World World;
    public UserInterface UserInterface;
    private ScGameplay ScGameplay { get; set; }
    private ScMenu ScMainMenu { get; set; }
    private ScMenu ScPauseMenu { get; set; }
    private Stack<GameScreen> ScreenStack { get; set; }

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.IsFullScreen = false;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        var config = JsonHelper.GetGameConfig();
        Config = config;
        _screenManager = new ScreenManager();
        ScreenStack = new Stack<GameScreen>();
        InitializeUserInterface(config);
        LoadMainMenuScreen();
        // LoadPauseMenuScreen(); <-- that's just for fun and test x)
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _screenManager.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        // UserInterface.DrawGameplay();
        _screenManager.Draw(gameTime);
    }

    // protected override void OnExiting(Object sender, EventArgs args)
    // {
    //     base.OnExiting(sender, args);
    //     World.Save("test_save");
    //     World._World.Dispose();
    // }

    public void LoadGameplayScreen(string saveFolderName = null)
    {
        if (ScGameplay is null) InitializeGameplay(Config, saveFolderName);
        LoadScreen(ScGameplay);
    }

    public void LoadMainMenuScreen()
    {
        if (ScMainMenu is null) InitializeMainMenu();
        LoadScreen(ScMainMenu);
    }

    public void LoadPauseMenuScreen()
    {
        if(ScPauseMenu is null) InitializePauseMenu();
        LoadScreen(ScPauseMenu);
    }

    public void LoadSaveMenuToSave()
    {
        LoadScreen(GetSaveMenu(toLoad: false));
    }

    public void LoadSaveMenuToLoad()
    {
        
        LoadScreen(GetSaveMenu(toLoad: true));
    }
    

    public void ReturnToPreviousScreen()
    {
        if (ScreenStack.Count < 2)
            throw new Exception("ScreenStack.Count is less than 2, so it cannot return to previous screen");
        ScreenStack.Pop();
        LoadScreen(ScreenStack.Pop());
    }

    private ScMenu GetSaveMenu(bool toLoad)
    {
        var saveButtons = toLoad
            ? UserInterface.GetSaveButtonsToLoadSave(Config)
            : UserInterface.GetSaveButtonsToMakeSave(Config);
        return new ScSaveList(this, _spriteBatch, saveButtons.ToList(), Config.LoadMenuBackgroundName,
            Config.LoadMenuBackgroundScale, Config);
    }

    private void InitializeUserInterface(ConfigInfo config)
    {
        UserInterface = new UserInterface(this, _spriteBatch, Content, config);
    }

    private void InitializeGameplay(ConfigInfo config, string saveFolderName = null)
    {
        ScGameplay = new ScGameplay(this, _spriteBatch, config, saveFolderName);
    }

    private void InitializeMainMenu()
    {
        UserInterface.InitializeMainMenuUI(Config);
        ScMainMenu = new ScMenu(this, _spriteBatch, UserInterface.MainMenuButtons, Config.MainMenuBackgroundName, Config.MainMenuBackgroundScale);
    }

    private void InitializePauseMenu()
    {
        UserInterface.InitializePauseMenuUI(Config);
        ScPauseMenu = new ScMenu(this, _spriteBatch, UserInterface.PauseMenuButtons, Config.MainMenuBackgroundName, Config.MainMenuBackgroundScale);
    }

    private void LoadScreen(GameScreen screen)
    {
        ScreenStack.Push(screen);
        _screenManager.LoadScreen(screen);
    }
}