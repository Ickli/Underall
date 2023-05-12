using System;
using Microsoft.Xna.Framework;

namespace Underall.Components;

public class CPosition: IComparable
{
    public Vector2 Vector;
    public int Depth;
    

    public CPosition(Vector2 vector, int depth)
    {
        Vector = vector;
        Depth = depth;
    }

    public int CompareTo(object obj)
    {
        if (obj is not CPosition anotherComponent)
            throw new ArgumentException("The object is not an CSizeAndPosition");
        return Depth.CompareTo(anotherComponent.Depth);
    }
}

public class CHitbox
{
    public int Width;
    public int Height;
}