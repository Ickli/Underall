using System;
// using System.Drawing;
using Microsoft.Xna.Framework;

namespace Underall.Components;

public class CSizePosition : IComparable
{
    public Vector2 Location;
    public int Width;
    public int Height;
    public int Depth;


    // public CSizePosition(Vector2 location, Vector2 dimensions, int depth)
    // {
    //     Location = location;
    //     Width = (int)dimensions.X;
    //     Height = (int)dimensions.Y;
    //     Depth = depth;
    // }
    //
    // public CSizePosition(Vector2 location, int width, int height, int depth)
    // {
    //     Location = location;
    //     Width = width;
    //     Height = height;
    //     Depth = depth;
    // }

    public bool Intersects(CSizePosition anotherComponent)
    {
        var points = GetCorners();
        var otherPoints = anotherComponent.GetCorners();

        return points[2].Y < otherPoints[0].Y || otherPoints[2].Y < points[0].Y ||
               points[2].X < otherPoints[0].X || otherPoints[2].X < points[0].X;
    }

    public int CompareTo(object obj)
    {
        if (obj is not CSizePosition anotherComponent)
            throw new ArgumentException("The object is not a CSizePosition");
        return Depth.CompareTo(anotherComponent.Depth);
    }


    /// <returns>array of points: top-left, top-right, bottom-right, bottom-left</returns>
    public Point[] GetCorners()
    {
        return new[]
        {
            new Point((int)Location.X, (int)Location.Y), // top left
            new Point((int)(Location.X + Width), (int)Location.Y), // top right
            new Point((int)(Location.X + Width), (int)(Location.Y + Height)), // bottom right
            new Point((int)Location.X, (int)(Location.Y + Height)), // bottom left
        };
    }
}