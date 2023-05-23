using Microsoft.Xna.Framework;

namespace Underall.Components;

public class CVelocity
{
    public Vector2 Vector;
    public bool IsFalling;
}

public class CWalkableBy
{
    public bool[] Dimensions; // true at index X means it is walkable by in dimension with X id
}