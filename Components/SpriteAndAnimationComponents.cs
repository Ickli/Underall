using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;

namespace Underall.Components;

// Sprite and AnimatedSprite cannot be serialized due to containing IntPtr instances
// so initialization happens after components' loading

public class CAnimatedSprite
{
    public string[] SpriteSheetNames;
    [JsonIgnore]
    public AnimatedSprite[] Sprites;
}

public class CSprite
{
    public string[] TextureNames;
    public int[] Xs;
    public int[] Ys;
    public int[] Widths;
    public int[] Heights;
    [JsonIgnore]
    public Sprite[] Sprites;
}