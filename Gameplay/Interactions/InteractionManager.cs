using System;
using MonoGame.Extended.Entities;
using Underall.Components;
using Underall.Helpers;

namespace Underall.Gameplay.Interactions;

public static class InteractionManager
{
    private static ComponentManager ComponentManager { get; set; }
    
    /// <summary>
    /// Must be used after all World initialization process
    /// </summary>
    public static void Initialize(World.World world)
    {
        ComponentManager = world.ComponentManager;
    }
    
    public static void ProcessInteraction(Interaction interaction)
    {
        switch (interaction.Type)
        {
            case InteractionType.RawAddBasicStats:
                ProcessRawAddBasicStats(interaction);
                break;
            default:
                throw new ArgumentException($"This type of interaction is not supported. Interaction: {interaction}");
        }
    }

    public static void ProcessReversedInteraction(Interaction interaction)
    {
        switch (interaction.Type)
        {
            case InteractionType.RawAddBasicStats:
                ReversedProcessAddBasicStats(interaction);
                break;
            default:
                throw new ArgumentException($"This type of interaction is not supported. Interaction: {interaction}");
        }
    }

    private static void ProcessRawAddBasicStats(Interaction interaction, ComponentMapper nonGenericMapper = null)
    {
        var objectStats = nonGenericMapper is null
            ? ComponentManager.GetMapper<CBasicStats>().Get(interaction.ObjectId)
            : ((ComponentMapper<CBasicStats>)nonGenericMapper).Get(interaction.ObjectId);
        var statNamesAmount = CBasicStats.StandardStatsNames.Length;
        
        for (var statNameIndex = 0; statNameIndex < statNamesAmount; statNameIndex++)
        {
            var statName = CBasicStats.StandardStatsNames[statNameIndex];
            if(interaction.Parameters.TryGetValue(statName, out var interactionStatValue) 
               && objectStats.TryGetValue(statName, out var objectStat))
                objectStat.Add((int)interactionStatValue);
        }
    }

    private static void ReversedProcessAddBasicStats(Interaction interaction, ComponentMapper nonGenericMapper = null)
    {
        ReverseBasicStatsParameters(interaction.Parameters);
        ProcessRawAddBasicStats(interaction, nonGenericMapper);
        ReverseBasicStatsParameters(interaction.Parameters);
    }

    private static void ReverseBasicStatsParameters(InteractionParameters parameters)
    {
        var statNamesAmount = CBasicStats.StandardStatsNames.Length;
        for (var statNameIndex = 0; statNameIndex < statNamesAmount; statNameIndex++)
        {
            var statName = CBasicStats.StandardStatsNames[statNameIndex];
            if (parameters.TryGetValue(statName, out var interactionStatValue))
                parameters[statName] = -(int)interactionStatValue;
        }
    }
}