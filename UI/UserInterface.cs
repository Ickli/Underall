using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Underall.Components;
using Underall.MetaInfo;

namespace Underall.Systems;

using Underall.UI;
using Underall.World;

public class UserInterface
{
    private World World { get; set; }
    private SpriteBatch SpriteBatch { get; set; }
    private int ControllableEntityId { get; set; }
    private CBasicStats ControllableBasicStats { get; set; }
    private CSizePosition ControllableSizePosition { get; set;}
    public StatBar HealthBar { get; set; }
    public StatBar StaminaBar { get; set; }
    private StatBar SanityBar { get; set; }

    public UserInterface(World world, SpriteBatch spriteBatch, ContentManager contentManager, ConfigInfo config)
    {
        /*
         1. assign world
         2. assign entity id
         3. fetch needed components of entity
         4. build GUI
         */
        World = world;
        SpriteBatch = spriteBatch;
        ControllableEntityId = world.ControllableEntityId;
        FetchControllableComponents(world.ComponentManager);
        BuildStatBars(SpriteBatch, world.ComponentManager, contentManager, config);
    }

    public void Draw()
    {
        SpriteBatch.Begin(SpriteSortMode.BackToFront);
        HealthBar.Draw();
        StaminaBar.Draw();
        SanityBar.Draw();
        SpriteBatch.End();
    }

    private void FetchControllableComponents(ComponentManager componentManager)
    {
        ControllableSizePosition = componentManager.GetMapper<CSizePosition>().Get(ControllableEntityId);
        ControllableBasicStats = componentManager.GetMapper<CBasicStats>().Get(ControllableEntityId);
    }

    private void BuildStatBars(SpriteBatch spriteBatch, ComponentManager componentManager, ContentManager contentManager, ConfigInfo config)
    {
        HealthBar = config.HealthBarBuilder
            .AddStatInfo(ControllableBasicStats.Health)
            .AddSpriteBatch(spriteBatch)
            .BindTexturesAndFonts(contentManager)
            .Build();
        SanityBar = config.SanityBarBuilder
            .AddStatInfo(ControllableBasicStats.Sanity)
            .AddSpriteBatch(spriteBatch)
            .BindTexturesAndFonts(contentManager)
            .Build();
        StaminaBar = config.StaminaBarBuilder
            .AddStatInfo(ControllableBasicStats.Stamina)
            .AddSpriteBatch(spriteBatch)
            .BindTexturesAndFonts(contentManager)
            .Build();
    }
}