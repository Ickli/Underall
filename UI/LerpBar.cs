using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Underall.UI;

/// <summary>
/// Base class for containing info about appearance of bar
/// </summary>
public class LerpBar
{
    [JsonIgnore]
    public Texture2D Texture;
    public string TextureName;
    public Vector2 Position;
    public Vector2 Scale;
    public Color[] MinMaxColors;
    protected static Vector2 Origin = new Vector2(0, 0);
    protected static int Depth = 0;

    public LerpBar(Texture2D texture, Vector2 position, Vector2 scale, Color[] minMaxColors)
    {
        Texture = texture;
        Position = position;
        Scale = scale;
        MinMaxColors = minMaxColors;
    }

    /// <summary>
    /// Draws bar on the screen with defined texture, position, scale and colors for lerping.
    /// It is assumed spriteBatch has already begun drawing.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, float amount)
    {
        spriteBatch.Draw(
            Texture,
            Position,
            null,
            Color.Lerp(MinMaxColors[0], MinMaxColors[1], amount),
            0,
            Origin,
            Scale,
            SpriteEffects.None,
            Depth
        );
    }
}