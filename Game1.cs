using System;
using System.IO;
using System.Text.Json;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Underall.Components;
using Underall.Helpers;
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
        var sphys = new SPhysics(config);
        var world = new WorldBuilder()
            .AddSystem(sphys)
            .AddSystem(new SControls(this, config))
            .Build();
        Components.Add(world);
        
        ComponentRegistry.InitializeComponentMappers(world);
        
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
        ent.Attach(new CSizePosition{Location = heroPos, Width = 45, Height = 45, Depth= 0});
        ent.Attach(new CControllable());

        // var manager = typeof(MonoGame.Extended.Entities.World).GetProperty("EntityManager",
        //     BindingFlags.NonPublic | BindingFlags.Instance).GetValue(world);
        // var bag = (Bag<Entity>)typeof(EntityManager).GetField("_entityBag", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(manager);
        // File.WriteAllText("/home/boogly/Documents/test.txt", JsonSerializer.Serialize(manager));
        
        var ent1 = world.CreateEntity();
        var ent2 = world.CreateEntity();
        Console.WriteLine(SaveHelper.GetJsonArrayFromEntities(world).ToJsonString());
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
        
        _spriteBatch.Draw(sprite, ent.Get<CSizePosition>().Location);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
