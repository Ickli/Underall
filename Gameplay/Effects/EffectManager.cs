using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Underall.Collections;
using MonoGame.Extended.Collections;
using Underall.Gameplay.Interactions;

namespace Underall.Gameplay.Effects;

using EffectHandler = EventHandler<GameplayArgs>;

public static class EffectManager
{
    private static SortedList<TimeSpan, Effect> Effects { get; set; }
    private static Dictionary<Effect, TimeSpan> EffectSpans { get; set; }
    private static Dictionary<InteractionType, EffectHandler> EffectHandlers { get; set; }
    private static List<int> ExpiredEffectIndices { get; set; }

    public static void Initialize()
    {
        Effects = new SortedList<TimeSpan, Effect>();
        EffectSpans = new Dictionary<Effect, TimeSpan>();
        InitializeEffectHandlers();
        ExpiredEffectIndices = new List<int>();
    }

    public static void LoadSavedEffects(GameTime time, SortedList<TimeSpan, Effect> savedEffects)
    {
        foreach(var timeEffect in savedEffects)
            RawAddEffect(time.TotalGameTime + timeEffect.Key, timeEffect.Value);
    }

    public static void Update(GameTime time)
    {
        for(var effectIndex = 0; effectIndex < Effects.Count; effectIndex++)
            if (time.TotalGameTime >= Effects.Keys[effectIndex])
                ExpiredEffectIndices.Add(effectIndex);
            else break;
        for(var expiredIndex = ExpiredEffectIndices.Count - 1; expiredIndex >= 0; expiredIndex--)
            RemoveEffectAt(expiredIndex);
    }

    public static void ProcessInteraction(object? sender, GameplayArgs args)
    {
        if (EffectHandlers.TryGetValue(args.Interaction.Type, out var handler))
            handler.Invoke(sender, args);
    }
    
    public static void AddEffect(GameTime time, Effect effect)
    {
        var index = Effects.IndexOfValue(effect);
        // dealing with RewritePolicy of oldEffect
        if(index >= 0 && Effects.Values[index] is Effect oldEffect)
        {
            switch (oldEffect.RewritePolicy)
            {
                case EffectRewritePolicy.Never:
                    return;
                case EffectRewritePolicy.Always:
                    break;
                case EffectRewritePolicy.Conditional:
                    var interaction = effect.Interaction;
                    if (oldEffect.RewriteCondition.IsSatisfiedBy(interaction.Parameters, interaction.SubjectId, interaction.ObjectId)) 
                        break;
                    break;
                default:
                    throw new NotImplementedException();
            }
            RemoveEffectAt(index);
        }
        RawAddEffect(time.TotalGameTime + effect.ExpirationSpan, effect);
    }

    private static void InitializeEffectHandlers()
    {
        EffectHandlers = new Dictionary<InteractionType, EffectHandler>();
        var interactionTypesAmount = Enum.GetNames<InteractionType>().Length;
        for(var type = 0; type < interactionTypesAmount; type++)
            EffectHandlers.Add((InteractionType)type, (sender, args) => {});
    }

    private static void RemoveEffect(Effect effect)
    {
        RemoveEffectAt(Effects.IndexOfValue(effect));
    }

    private static void RemoveEffectAt(int index)
    {
        var effect = Effects.Values[index];
        Effects.RemoveAt(index);
        EffectSpans.Remove(effect);
        RemoveEffectHandlers(effect);
    }

    private static void RawAddEffect(TimeSpan time, Effect effect)
    {
        Effects.Add(time, effect);
        EffectSpans.Add(effect, time);
        AddEffectHandlers(effect);
    }

    private static void AddEffectHandlers(Effect effect)
    {
        var handlers = effect.GetHandlers();
        for (var handlerIndex = 0; handlerIndex < handlers.Count; handlerIndex++)
            EffectHandlers[effect.Interaction.Type] += handlers[handlerIndex];
    }

    private static void RemoveEffectHandlers(Effect effect)
    {
        var handlers = effect.GetHandlers();
        for (var handlerIndex = 0; handlerIndex < handlers.Count; handlerIndex++)
            RemoveEffectHandler(effect.Interaction.Type, handlers[handlerIndex]);
    }

    public static void RemoveEffectHandler(InteractionType effectType, EffectHandler handler) =>
        EffectHandlers[effectType] -= handler;

    public static SortedList<TimeSpan, Effect> GetEffectsWithTimeSpan(GameTime time)
    {
        var list = new SortedList<TimeSpan, Effect>();
        
        foreach(var pair in Effects)
            list.Add(pair.Key - time.TotalGameTime, pair.Value);
        return list;
    }

    /// <summary>
    /// Doesn't check if effect is contained in EffectManager
    /// </summary>
    public static void ShortenEffectTime(Effect effect, TimeSpan spanShortening) => EffectSpans[effect] -= spanShortening;
   
    /// <summary>
    /// Doesn't check if effect is contained in EffectManager
    /// </summary>
    public static void ExtendEffectTime(Effect effect, TimeSpan spanExtending) => EffectSpans[effect] += spanExtending;

    // the idea was to use Dictionary<InteractionType, EffectEvent> and just invoke multicasted to it lambda expressions
    // that are taken right from Effect.
    // private static void AddHandlers(Effect effect)
    // {
    //      
    // }
}