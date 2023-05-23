using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using Underall.Components;

namespace Underall.UI;

/// <summary>
/// Class that is fetched from game_config.json during initialization and needed
/// and needed for convenient StatBar creation.
/// </summary>
public class StatBarBuilder
{
    [JsonIgnore] 
    private SpriteBatch SpriteBatch;
    [JsonIgnore] 
    private SpriteFont SpriteFont;
    [JsonIgnore]
    private StatInfo StatInfo;

    public string SpriteFontName;
    public LerpBar LerpBar;
    // public string IndicatorValue;
    // public Vector2 IndicatorPosition;
    public Vector2 IndicatorScale;
    public Color IndicatorColor;

    public StatBarBuilder AddSpriteBatch(SpriteBatch spriteBatch)
    {
        SpriteBatch = spriteBatch;
        return this;
    }

    public StatBarBuilder AddStatInfo(StatInfo statInfo)
    {
        StatInfo = statInfo;
        return this;
    }

    public StatBarBuilder BindTexturesAndFonts(ContentManager contentManager)
    {
        LerpBar.Texture = contentManager.Load<Texture2D>(LerpBar.TextureName);
        SpriteFont = contentManager.Load<SpriteFont>(SpriteFontName);
        return this;
    }

    public StatBar Build()
    {
        return new StatBar(SpriteBatch, 
            StatInfo, 
            SpriteFont, 
            IndicatorScale, 
            IndicatorColor, 
            LerpBar.Texture,
            LerpBar.Position, 
            LerpBar.Scale,
            LerpBar.MinMaxColors
            );
    }
}