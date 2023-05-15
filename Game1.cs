using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;
using Underall.Components;
using Underall.Helpers;
using Underall.MetaInfo;

namespace Underall;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private string CurrentSave;

    private Sprite sprite;
    public int timeDif = 5;
    public Entity ent;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        var config = JsonHelper.GetGameConfig();
        var world = new Underall.World.World(this, "test_save", config);
        
        ent = world._World.GetEntity(0);
        sprite = world.ComponentManager.GetMapper<CSprite>().Components[0].Sprite;
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
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        _spriteBatch.Begin();
        
        _spriteBatch.Draw(sprite, ent.Get<CSizePosition>().Location);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
