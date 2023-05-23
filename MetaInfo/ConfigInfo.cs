using Microsoft.Xna.Framework;
using Underall.UI;

namespace Underall.MetaInfo;

public record ConfigInfo
{
    public int PositiveVelocityLimit;
    public int NegativeVelocityLimit;
    public int DefaultLayeredGridDepth;
    public float EpsilonVelocity;
    public StatBarBuilder HealthBarBuilder;
    public StatBarBuilder SanityBarBuilder;
    public StatBarBuilder StaminaBarBuilder;
}