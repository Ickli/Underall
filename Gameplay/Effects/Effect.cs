using System;
using MonoGame.Extended.Collections;
using Underall.Gameplay.Interactions;

namespace Underall.Gameplay.Effects;

using EffectHandler = EventHandler<GameplayArgs>;

public class Effect
{
    // there is no guarantee that Effect is not bound to entity's blueprint
    // so it is not acceptable to change ExpirationSpan of the effect
    public readonly TimeSpan ExpirationSpan;
    public readonly Interaction Interaction;

    public readonly EventHook ToStart; // after triggering the hook, effect is applied
    public readonly EventHook ToShorten; // after triggering the hook, effect's time is shortened
    public readonly EventHook ToExtend; // after triggering the hook, effect's time is extended
    public readonly EventHook ToEnd; // after triggering the hook, effect is removed

    public readonly TimeSpan SpanShortening;
    public readonly TimeSpan SpanExtending; 
    
    public readonly EffectRewritePolicy RewritePolicy; // describes when the effect can be rewritten by another one
    public readonly Condition RewriteCondition; // describes what conditions should be met by new effect to rewrite this one

    // public void ProcessInvokedInteraction(ref DateTime time, Interaction interaction)
    // {
    //     // think about order and else-if construction to optimize this
    //     if(ToStart.Condition.IsSatisfiedBy(interaction.Parameters, interaction.SubjectId, interaction.ObjectId)) 
    //         InteractionManager.ProcessInteraction(Interaction);
    //     if (ToShorten.Condition.IsSatisfiedBy(interaction.Parameters, interaction.SubjectId, interaction.ObjectId)) 
    //         time -= SpanShortening;
    //     if (ToExtend.Condition.IsSatisfiedBy(interaction.Parameters, interaction.SubjectId, interaction.ObjectId)) 
    //         time += SpanExtending;
    //     if(ToEnd.Condition.IsSatisfiedBy(interaction.Parameters, interaction.SubjectId, interaction.ObjectId)) 
    //         InteractionManager.ProcessReversedInteraction(Interaction);
    // }
    
    public Bag<EffectHandler> GetHandlers()
    {
        var bag = new Bag<EffectHandler>();
        if(ToStart != null) bag.Add(ToStartHandler);
        if(ToShorten != null) bag.Add(ToShortenHandler);
        if(ToExtend != null) bag.Add(ToExtendHandler);
        if(ToEnd != null) bag.Add(ToEndHandler);
        return bag;
    }
    
    private void ToStartHandler(object? sender, GameplayArgs args)
    {
        if (!ToStart.Condition.IsSatisfiedBy(args.Interaction.Parameters, args.Interaction.SubjectId,
                args.Interaction.ObjectId))
            return;
        InteractionManager.ProcessInteraction(args.Interaction);
        if(ToStart.RemoveAfterTrigger) EffectManager.RemoveEffectHandler(Interaction.Type, ToStartHandler);
    }
    
    private void ToShortenHandler(object? sender, GameplayArgs args)
    {
        if (!ToShorten.Condition.IsSatisfiedBy(args.Interaction.Parameters, args.Interaction.SubjectId, args.Interaction.ObjectId))
            return;
        EffectManager.ShortenEffectTime(this, SpanShortening);
        if(ToShorten.RemoveAfterTrigger) EffectManager.RemoveEffectHandler(Interaction.Type, ToShortenHandler);
    }
    
    private void ToExtendHandler(object? sender, GameplayArgs args)
    {
        if (!ToExtend.Condition.IsSatisfiedBy(args.Interaction.Parameters, args.Interaction.SubjectId, args.Interaction.ObjectId))
            return;
        EffectManager.ExtendEffectTime(this, SpanExtending);
        if(ToExtend.RemoveAfterTrigger) EffectManager.RemoveEffectHandler(Interaction.Type, ToExtendHandler);
    }
    
    private void ToEndHandler(object? sender, GameplayArgs args)
    {
        if (!ToEnd.Condition.IsSatisfiedBy(args.Interaction.Parameters, args.Interaction.SubjectId, args.Interaction.ObjectId))
            return;
        InteractionManager.ProcessReversedInteraction(args.Interaction);
        if(ToEnd.RemoveAfterTrigger) EffectManager.RemoveEffectHandler(Interaction.Type, ToEndHandler);
    }

    public override int GetHashCode() => Interaction.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is not Effect effect) return false;
        return ReferenceEquals(this, effect) || GetHashCode().Equals(effect.GetHashCode());
    }
}

public enum EffectRewritePolicy
{
    Always,
    Conditional,
    Never
}