using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Underall.Collections;
using Underall.Components;
using Underall.Gameplay.Interactions;
using Underall.Helpers;

namespace Underall.Gameplay.Effects;

public class Condition
{
    // NotInitializedId is used when you don't need any Subject/Object id matching
    // and you are bothered only with stats and stuff.
    private const int NotInitializedId = CMeta.NotInitializedId;
    private static readonly float Epsilon = MathHelper.Epsilon;

    public readonly int SubjectId;

    public readonly int ObjectId;
    // array of pairs where First is StatName, Second is ConditionType, Third is StatValue
    public AssociativeArray<string, ConditionType, object> ParameterConditions;

    public bool IsSatisfiedBy(Dictionary<string, object> parameters, int subjectId = NotInitializedId, int objectId = NotInitializedId)
    {
        // checking for particular subject and object entities 
        if ((SubjectId != NotInitializedId && SubjectId != subjectId) ||
            (ObjectId != NotInitializedId && ObjectId != objectId))
            return false;

        for (var conditionIndex = 0; conditionIndex < ParameterConditions.First.Length; conditionIndex++)
        {
            var paramName = ParameterConditions.First[conditionIndex];
            var condType = ParameterConditions.Second[conditionIndex];
            var conditionValue = ParameterConditions.Third[conditionIndex];
            if (!parameters.TryGetValue(paramName, out var interactionValue)) 
                return false;
            
            switch (condType)
            {
                case ConditionType.AmountEquals:
                    if (Epsilon < Math.Abs((float)conditionValue - (float)interactionValue)) return false;
                    break;
                case ConditionType.AmountLess:
                    if ((float)conditionValue < (float)interactionValue) return false;
                    break;
                case ConditionType.AmountMore:
                    if ((float)conditionValue > (float)interactionValue) return false;
                    break;
                case ConditionType.AmountEqualsLess:
                    if (-Epsilon < (float)conditionValue - (float)interactionValue) return false;
                    break;
                case ConditionType.AmountEqualsMore:
                    if (Epsilon > (float)conditionValue - (float)interactionValue) return false;
                    break;
            }
        }
        // if none of conditions has failed
        return true;
    }
}

/// <summary>
/// Describes what of interaction should be regarded to activate EventHook.
/// </summary>
/// <remarks>
/// InteractionType is always in account so it is not included.
/// AmountLess means interaction stat's amount is less than conditional one etc.
/// Using Amount- conditions with non-numeric stats must throw an exception at a manager level.
/// </remarks>
public enum ConditionType
{
    AmountEquals,
    AmountLess,
    AmountMore,
    AmountEqualsLess,
    AmountEqualsMore,
}