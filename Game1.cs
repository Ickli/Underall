using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;
using Underall.Components;
using Underall.Helpers;
using Underall.MetaInfo;
using Underall.Systems;

namespace Underall;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private string CurrentSave;

    private Sprite sprite;
    public int timeDif = 5;
    public Entity ent;
    public Entity ent1;
    private World.World World;
    private UserInterface UserInterface;

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
        World = new Underall.World.World(this, GraphicsDevice, _spriteBatch, "test_save", config);
        
        ent = World._World.GetEntity(0);
        sprite = World.ComponentManager.GetMapper<CSprite>().Components[0].Sprites[0];
        
        InitializeUserInterface(config);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        UserInterface.Draw();
    }

    // protected override void OnExiting(Object sender, EventArgs args)
    // {
    //     base.OnExiting(sender, args);
    //     World.Save("test_save");
    //     World._World.Dispose();
    // }

    private void InitializeUserInterface(ConfigInfo config)
    {
        UserInterface = new UserInterface(World, _spriteBatch, Content, config);
    }
}
