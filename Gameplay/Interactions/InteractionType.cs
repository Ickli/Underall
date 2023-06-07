namespace Underall.Gameplay.Interactions;

/// <summary>
/// Types of interactions that entities can have between each other.
/// </summary>
/// <remarks>
/// Prefix Raw- means that entity is very likely not to respond to this Raw-InteractionType by another interaction.
/// You should not redefine values of the enum.
/// Every type must be handled in methods:
///     Condition.Satisfies, InteractionManager.ParseInteraction, InteractionManager.ProcessReversedInteraction
/// </remarks>
public enum InteractionType
{
    RawAddBasicStats,
}