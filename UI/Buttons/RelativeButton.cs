using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Underall.Helpers;
using Underall.Systems;
using Underall.UI.Frames;
using Underall.UI.Interfaces;


namespace Underall.UI.Buttons;

public class RelativeButton: RelativeFrame, IHoverable, IClickable, ILabeled
{
    private static string _stringForEmptyButton = "";
    private static Vector2 _notScalingVector = new Vector2(1,1);
    
    #region IHoverable
    [JsonIgnore] public Texture2D OnHoverTexture { get; set; }
    [JsonIgnore] public SoundEffect OnHoverSoundEffect { get; set; }
    public string OnHoverTextureName { get; set; }
    public string OnHoverSoundEffectName { get; set; }
    #endregion
    
    #region IClickable
    [JsonIgnore] public Action<UIEventArgs> OnClickFunction { get; set; }
    [JsonIgnore] public Texture2D OnClickTexture { get; set; }
    [JsonIgnore] public SoundEffect OnClickSoundEffect { get; set; }
    public string OnClickFunctionName { get; set; }
    public string OnClickTextureName { get; set; }
    public string OnClickSoundEffectName { get; set; }
    #endregion
    
    // label is the last RelativeFrameTextItem
    #region ILabeled

    public string Label => TextItems.Count == 0 ? "" : TextItems.Last().Text;

    #endregion
    
    public override void BindSource(ContentManager contentManager)
    {
        base.BindSource(contentManager);
        ((IHoverable)this).BindSourceHoverable(contentManager);
        ((IClickable)this).BindSourceClickable(contentManager);
    }

    public void DrawHovered()
    {
        ((IHoverable)this).DrawHovered(SpriteBatch, AbsoluteOffset, Color, Scale, Depth);
        foreach(var item in TextItems)
            item.Draw(SpriteBatch, AbsoluteOffset);
    }

    public RelativeButton Initialized(UserInterface userInterface, SpriteBatch spriteBatch, ContentManager contentManager, Vector2 parentDimensions)
    {
        base.Initialize(userInterface, spriteBatch, contentManager, parentDimensions);
        return this;
    }

    public RelativeButton WithUVPosition(Vector2 uvPosition, Vector2 parentDimensions)
    {
        UVPosition = uvPosition;
        base.ComputeAbsoluteOffsets(parentDimensions);
        return this;
    }
    
    public RelativeButton WithNewTextItem(string text, RelativeFrameTextItem standardTextItem, Vector2? textScale = null)
    {
        var scale = textScale ?? _notScalingVector;
        TextItems.Add(standardTextItem.WithText(text).WithTextScale(scale));
        return this;
    }

    /// <summary>
    /// TextItems must be initialized and have at least one RelativeFrameTextItem
    /// </summary>
    /// <returns></returns>
    public RelativeButton WithReplacedLabel(string newLabel)
    {
        TextItems.Last().Text = newLabel;
        return this;
    }
    
    public RelativeButton WithOnClickFunction(Action<UIEventArgs> function)
    {
        OnClickFunction = function;
        return this;
    }

    public RelativeButton WithOnClickFunction(string functionName)
    {
        return WithOnClickFunction(UIFunctionRegistry.Functions[functionName]);
    }
}