﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Underall.Components;
using Underall.MetaInfo;
using Underall.Systems;

namespace Underall;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    private SaveInfo CurrentSave;
    private SControls _SControls;

    private GameTime time = new GameTime();
    
    
    private SpriteFont spriteFont;
    private Sprite sprite;
    public Vector2 heroPos = new Vector2(50, 50);
    public int timeDif = 5;
    public Vector2 heroSpeed = new Vector2(0, 0);
    public Entity ent;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
        
        
        var config = new ConfigInfo
        {
            PositiveVelocityLimit = 10, 
            NegativeVelocityLimit = -10,
            ControllablePositiveVelocityLimit = 10,
            ControllableNegativeVelocityLimit = -10
        };
        var world = new WorldBuilder()
            .AddSystem(new SPhysics(config))
            .AddSystem(new SControls(this, config))
            .Build();
        Components.Add(world);
        
        var ent = world.CreateEntity();
        this.ent = ent;
        sprite = new Sprite(new TextureRegion2D(Content.Load<Texture2D>("kindpng_988229"), 0, 0, 45, 45));
        ent.Attach(new CSprite
        {
            Sprite = sprite
        });
        ent.Attach(new CVelocity
        {
            Vector = heroSpeed
        });
        ent.Attach(new CPosition(heroPos, 0));
        ent.Attach(new CControllable());
        
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // spriteFont = Content.Load<SpriteFont>("Sprites");
        // sprite = Content.Load<Texture2D>("kindpng_988229");
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        _spriteBatch.Begin();
        
        _spriteBatch.Draw(sprite, ent.Get<CPosition>().Vector);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
