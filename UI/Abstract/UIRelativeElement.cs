using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Underall.UI.Abstract;

public abstract class UIRelativeElement
{
    public Vector2 UVPosition = new Vector2(0,0);
    public Vector2 Scale = new Vector2(1,1);
    public Color Color = Color.White;
    public string SourceName;
    [JsonIgnore] public Vector2 AbsoluteOffset;

    /// <summary>
    /// Gets source from source name
    /// </summary>
    public virtual void BindSource(ContentManager contentManager) {}

    public void ComputeAbsoluteOffset(Vector2 parentDimensions) => 
        AbsoluteOffset = Vector2.Multiply(parentDimensions, UVPosition);
}