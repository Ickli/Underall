using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Content;
using Underall.Systems;
using Underall.UI.Abstract;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Underall.UI.Frames;

/// <summary>
/// Frame is used for showing batch of text and images aligned by UV coordinates
/// </summary>
public class RelativeFrame: UIRelativeElement
{
    private static Vector2 _origin = new Vector2(0, 0);
    protected static int Depth = 0;
    [JsonIgnore] protected SpriteBatch SpriteBatch;
    [JsonIgnore] public Texture2D Texture;
    [JsonIgnore] public Vector2 Dimensions;
    [JsonIgnore] public UserInterface UserInterface;
    [JsonInclude] public List<RelativeFrameTextItem> TextItems { get; protected set; }
    [JsonInclude] public List<RelativeFrameTextureItem> TextureItems { get; protected set; }

    public void Initialize(UserInterface userInterface, SpriteBatch spriteBatch, ContentManager contentManager, Vector2 parentDimensions)
    {
        UserInterface = userInterface;
        SpriteBatch = spriteBatch;
        BindSource(contentManager);
        Dimensions = GetDimensions();
        ComputeAbsoluteOffsets(parentDimensions);
        foreach (var item in TextItems)
            item.Frame = this;
        foreach(var item in TextureItems)
            item.Frame = this;
    }
    
    public virtual void ComputeAbsoluteOffsets(Vector2 parentDimensions)
    {
        base.ComputeAbsoluteOffset(parentDimensions);
        var frameDimensions = GetDimensions();
        foreach (var item in TextItems)
            item.ComputeAbsoluteOffset(frameDimensions);
        foreach(var item in TextureItems)
            item.ComputeAbsoluteOffset(frameDimensions);
    }

    public virtual void BindSource(ContentManager contentManager)
    {
        Texture = contentManager.Load<Texture2D>(SourceName);
        foreach (var item in TextItems)
            item.BindSource(contentManager);
        foreach(var item in TextureItems)
            item.BindSource(contentManager);
    }

    /// <summary>
    /// It is assumed spriteBatch has already begun drawing and sources are bound
    /// </summary>
    public void Draw(Vector2 framePosition)
    {
        // draw frame
        SpriteBatch.Draw(
            Texture,
            framePosition,
            null,
            Color,
            0,
            _origin,
            Scale,
            SpriteEffects.None,
            Depth
        );
        // draw texture items
        for (var itemIndex = 0; itemIndex < TextureItems.Count; itemIndex++)
            TextureItems[itemIndex].Draw(SpriteBatch, framePosition);
        // draw text items
        for(var itemIndex = 0; itemIndex < TextItems.Count; itemIndex++)
            TextItems[itemIndex].Draw(SpriteBatch, framePosition);
    }

    public Vector2 GetDimensions() => Vector2.Multiply(Scale, new Vector2(Texture.Width, Texture.Height));
}

/// <summary>
/// FrameItem contains image and text that are aligned by TextureUVPosition and TextUVPosition accordingly.
/// </summary>
public class RelativeFrameTextureItem: RelativeFrameItem
{
    private static Vector2 _origin = new Vector2(0, 0);
    private static int Depth = 0;
    [JsonIgnore] public Texture2D Texture;

    /// <summary>
    /// It is assumed spriteBatch has already begun drawing and sources are bound.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, Vector2 framePosition)
    {
        spriteBatch.Draw(
            Texture,
            framePosition + AbsoluteOffset,
            null,
            Color,
            0,
            _origin,
            Scale,
            SpriteEffects.None,
            Depth
        );
    }
    
    public override void BindSource(ContentManager contentManager) =>
        Texture = contentManager.Load<Texture2D>(SourceName);
}

public class RelativeFrameTextItem: RelativeFrameItem
{
    private static int Depth = 0;
    private static Vector2 _origin = new Vector2(0, 0);
    [JsonIgnore] public SpriteFont SpriteFont;
    public string Text;
    
    /// <summary>
    /// It is assumed spriteBatch has already begun drawing and sources are bound.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, Vector2 framePosition)
    {
        spriteBatch.DrawString(
            SpriteFont,
            Text,
            framePosition + AbsoluteOffset,
            Color,
            0,
            _origin,
            Scale,
            SpriteEffects.None,
            Depth
            );
    }

    public override void BindSource(ContentManager contentManager) =>
        SpriteFont = contentManager.Load<SpriteFont>(SourceName);

    public RelativeFrameTextItem WithText(string text)
    {
        Text = text;
        return this;
    }

    public RelativeFrameTextItem WithTextScale(Vector2 scale)
    {
        Scale = scale;
        return this;
    }
}

public abstract class RelativeFrameItem: UIRelativeElement
{
    [JsonIgnore] public RelativeFrame Frame;
}