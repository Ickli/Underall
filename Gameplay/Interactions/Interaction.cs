using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Audio;
using Underall.Helpers;

namespace Underall.Gameplay.Interactions;


/// <summary>
/// Interactions that entities can have between each other.
/// </summary>
public class Interaction
{
    public readonly InteractionType Type;
    // Actual sound is dispatched in SoundManager
    public readonly string SoundName;
    public readonly int SubjectId;
    public readonly int ObjectId;
    public readonly InteractionParameters Parameters;

    public bool IsSoundDefined() => SoundName != JsonHelper.EmptySourceName;

    public override string ToString()
    {
        return $"Type: {Type}, SoundName: {SoundName}, Sound initialized: {IsSoundDefined()}, SubjectId: {SubjectId}, " +
               $"ObjectId: {ObjectId}, Parameters: {{{Parameters}}}";
    }

    public override int GetHashCode()
    {
        return Parameters.GetHashCode() ^ Type.GetHashCode();
    }

    public bool Equals(Interaction other) => this.Type == other.Type && Parameters.Equals(other.Parameters);
}

/// <summary>
/// Dictionary of parameters dedicated for Interaction.
/// Doesn't apply any restrictions on values:
/// if the actual type is inappropriate you will get exception in InteractionManager
/// </summary>
public class InteractionParameters : Dictionary<string, object>
{
    public override string ToString() =>
        this
            .Select(kvPair => $"{kvPair.Key}: {kvPair.Value}, ")
            .Aggregate(new StringBuilder(), (builder, str) => builder.Append(str))
            .ToString();

    public override int GetHashCode() => 
        Keys.Select(key => key.GetHashCode()).Aggregate((hash, key) => hash ^ key);

    public bool Equals(InteractionParameters other) => this.Count == other.Count && !this.Except(other).Any();
}