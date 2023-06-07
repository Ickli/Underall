using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Underall.Gameplay.Sound;

/// <summary>
/// Controls sound during menu and gameplay screens 
/// </summary>
// It doesn't control menu sounds yet. TODO: Make UI sound depend on SoundManager
public static class SoundManager
{
    private static ContentManager ContentManager { get; set; }
    private static Dictionary<string, SoundEffect> SoundCache { get; set; }

    public static void Initialize(ContentManager contentManager)
    {
        ContentManager = contentManager;
        SoundCache = new Dictionary<string, SoundEffect>();
    }
    
    public static SoundEffect GetOrCreateSoundEffect(string soundName)
    {
        if (SoundCache.TryGetValue(soundName, out var effect)) return effect;
        effect = ContentManager.Load<SoundEffect>(soundName);
        SoundCache.Add(soundName, effect);
        return effect;
    }

    public static void Play(string soundName) => GetOrCreateSoundEffect(soundName).Play();
}