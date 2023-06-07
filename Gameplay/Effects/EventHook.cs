namespace Underall.Gameplay.Effects;

/// <summary>
/// Describes what have to happen to get Effect started/collapsed/shortened/extended
/// </summary>
public class EventHook
{
    public readonly Condition Condition;
    public readonly bool RemoveAfterTrigger;
}