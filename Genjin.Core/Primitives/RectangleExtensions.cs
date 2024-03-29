﻿// MIT-licensed (https://github.com/craftworkgames/MonoGame.Extended)

using System.Numerics;

namespace Genjin.Core.Primitives; 

public static class RectangleExtensions
{
    /// <summary>
    ///     Gets the corners of the rectangle in a clockwise direction starting at the top left.
    /// </summary>
    public static Vector2[] GetCorners(this Box rectangle)
    {
        var corners = new Vector2[4];
        corners[0] = new Vector2(rectangle.Left, rectangle.Top);
        corners[1] = new Vector2(rectangle.Right, rectangle.Top);
        corners[2] = new Vector2(rectangle.Right, rectangle.Bottom);
        corners[3] = new Vector2(rectangle.Left, rectangle.Bottom);
        return corners;
    }


    public static Box Clip(this Box rectangle, Box clippingRectangle)
    {
        var clip = clippingRectangle;
        rectangle.X = clip.X > rectangle.X ? clip.X : rectangle.X;
        rectangle.Y = clip.Y > rectangle.Y ? clip.Y : rectangle.Y;
        rectangle.Width = rectangle.Right > clip.Right ? clip.Right - rectangle.X : rectangle.Width;
        rectangle.Height = rectangle.Bottom > clip.Bottom ? clip.Bottom - rectangle.Y : rectangle.Height;

        if(rectangle.Width <= 0 || rectangle.Height <= 0)
            return Box.Empty;

        return rectangle;
    }
}
