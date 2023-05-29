using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Sprites;
using Underall.Components;
using Underall.Helpers;
using Underall.MetaInfo;
using Underall.Systems;

namespace Underall.Screens;

public class ScGameplay: GameScreen
{
    private SpriteBatch SpriteBatch { get; set; }
    private string CurrentSaveName { get; set; }
    private World.World World { get; set; }
    private new Game1 Game => (Game1)base.Game;
    
    
    public ScGameplay(Game1 game, SpriteBatch spriteBatch, ConfigInfo config, string saveFolderName = null) : base(game)
    {
        base.Initialize();
        SpriteBatch = spriteBatch;
        World = new Underall.World.World(Game, GraphicsDevice, SpriteBatch, saveFolderName ?? config.DefaultLevelName, config);
        CurrentSaveName = saveFolderName;
        Game.UserInterface.InitializeGameplayUI(config);
    }

    public override void LoadContent()
    {
        Game.Components.Add(World._World);
    }

    public override void UnloadContent()
    {
        Game.Components.Remove(World._World);
    }
    
    public override void Update(GameTime gameTime)
    {
        // Entities are updated inside _World systems
        // So it is enough to only add _World to Game.Components with systems initialized.
    }

    public override void Draw(GameTime gameTime)
    {
        // Entities on the screen are drawn inside SDraw system
        // which is in World._World,
        // So it is enough to only add _World to Game.Components with systems initialized.
        Game.UserInterface.DrawGameplay();
    }
}

