using System;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Underall.Components;

namespace Underall.UI.Bars;


public class StatBar: LerpBar
{
    [JsonIgnore] private SpriteFont SpriteFont { get; set; }
    [JsonIgnore] private SpriteBatch SpriteBatch { get; set; }
    [JsonIgnore]
    public StatInfo StatInfo { get; set; }
    private string IndicatorValue { get; set; }
    private Vector2 IndicatorPosition { get; set; }
    private Vector2 IndicatorScale { get; set; }
    private Color IndicatorColor { get; set; }
    
    public StatBar(
        SpriteBatch spriteBatch,
        StatInfo statInfo, 
        SpriteFont spriteFont, 
        Vector2 indicatorScale, 
        Color indicatorColor, 
        Texture2D texture, 
        Vector2 position, 
        Vector2 scale, 
        Color[] minMaxColors)
        : base(texture, position, scale, minMaxColors)
    {
        SpriteBatch = spriteBatch;
        StatInfo = statInfo;
        SpriteFont = spriteFont;
        StatInfo.StatChanged += UpdateIndicatorValue;
        IndicatorScale = indicatorScale;
        IndicatorPosition = Position;
        IndicatorColor = indicatorColor;
        UpdateIndicatorValue();
    }

    public void Draw()
    {
        base.Draw(SpriteBatch, StatInfo.CurrentProportion);
        // Draw Current from StatInfo
        SpriteBatch.DrawString(
            SpriteFont,
            IndicatorValue,
            IndicatorPosition,
            IndicatorColor,
            0,
            Origin,
            IndicatorScale,
            SpriteEffects.None,
            Depth
            );
    }
    public void UpdateIndicatorValue() => IndicatorValue = StatInfo.Current.ToString();
}