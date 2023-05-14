using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;

namespace Underall.Components;

// Sprite and AnimatedSprite cannot be serialized due to containing IntPtr instances
// so initialization happens after components' loading

public class CAnimatedSprite
{
    public string SpriteSheetName;
    [JsonIgnore]
    public AnimatedSprite Sprite;
}

public class CSprite
{
    public string TextureName;
    public int X;
    public int Y;
    public int Width;
    public int Height;
    [JsonIgnore]
    public Sprite Sprite;
}