using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Underall.UI.Interfaces;

public interface IHoverable
{
    private static Vector2 _origin = new Vector2(0, 0);
    [JsonIgnore] public Texture2D OnHoverTexture { get; set; }
    [JsonIgnore] public SoundEffect OnHoverSoundEffect { get; set; }
    public string OnHoverTextureName { get; set; }
    public string OnHoverSoundEffectName { get; set; }

    public void BindSourceHoverable(ContentManager contentManager)
    {
        if(OnHoverTextureName != "")
            OnHoverTexture = contentManager.Load<Texture2D>(OnHoverTextureName);
        if(OnHoverSoundEffectName != "")
            OnHoverSoundEffect = contentManager.Load<SoundEffect>(OnHoverSoundEffectName);
    }

    public bool IsHovered(Vector2 hoverablePosition, int hoverableWidth, int hoverableHeight, Point mousePosition)
    {
        return mousePosition.X >= hoverablePosition.X
               && mousePosition.X <= hoverablePosition.X + hoverableWidth
               && mousePosition.Y >= hoverablePosition.Y
               && mousePosition.Y < hoverablePosition.Y + hoverableHeight;
    }

    public void DrawHovered(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 scale, int depth)
    {
        spriteBatch.Draw(
            OnHoverTexture,
            position,
            // new Vector2(20, 0),
            null,
            color,
            0,
            _origin,
            scale,
            SpriteEffects.None,
            depth
        );
    }
}