using System;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Underall.UI.Interfaces;

public interface IClickable
{
    [JsonIgnore] public Action<UIEventArgs> OnClickFunction { get; set; }
    [JsonIgnore] public Texture2D OnClickTexture { get; set; }
    [JsonIgnore] public SoundEffect OnClickSoundEffect { get; set; }
    public string OnClickFunctionName { get; set; }
    public string OnClickTextureName { get; set; }
    public string OnClickSoundEffectName { get; set; }
    
    public void BindSourceClickable(ContentManager contentManager)
    {
        if(OnClickFunctionName != "")
            OnClickFunction = UIFunctionRegistry.Functions[OnClickFunctionName];
        if(OnClickTextureName != "")
            OnClickTexture = contentManager.Load<Texture2D>(OnClickTextureName);
        if(OnClickSoundEffectName != "")
            OnClickSoundEffect = contentManager.Load<SoundEffect>(OnClickSoundEffectName);
    }

    public void Click(Game1 game, string saveName = null)
    {
        OnClickFunction(new UIEventArgs(this, game));
    }
}