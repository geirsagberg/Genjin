// MIT-licensed (https://github.com/craftworkgames/MonoGame.Extended)

using System.Numerics;

namespace Genjin.Core.Primitives; 

/// <summary>
///     Base class for shapes.
/// </summary>
/// <remakarks>
///     Created to allow checking intersection between shapes of different types.
/// </remakarks>
public interface IShapeF
{
    RectangleF BoundingRectangle { get; }
    Vector2 Position { get; set; }
    Size2F Size { get; }
}

/// <summary>
///     Class that implements methods for shared <see cref="IShapeF" /> methods.
/// </summary>
public static class Shape
{
    /// <summary>
    ///     Check if two shapes intersect.
    /// </summary>
    /// <param name="shapeA">The first shape.</param>
    /// <param name="shapeB">The second shape.</param>
    /// <returns>True if the two shapes intersect.</returns>
    public static bool Intersects<T1, T2>(this T1 shapeA, T2 shapeB) where T1: IShapeF where T2: IShapeF =>
        shapeA switch {
            RectangleF rectangleA when shapeB is RectangleF rectangleB => rectangleA.Intersects(rectangleB),
            CircleF circleA when shapeB is CircleF circleB => circleA.Intersects(circleB),
            RectangleF rect1 when shapeB is CircleF circ1 => Intersects(circ1, rect1),
            CircleF circ2 when shapeB is RectangleF rect2 => Intersects(circ2, rect2),
            _ => false
        };

    /// <summary>
    ///     Checks if a circle and rectangle intersect.
    /// </summary>
    /// <param name="circle">Circle to check intersection with rectangle.</param>
    /// <param name="rectangle">Rectangle to check intersection with circle.</param>
    /// <returns>True if the circle and rectangle intersect.</returns>
    public static bool Intersects(CircleF circle, RectangleF rectangle)
    {
        var closestPoint = rectangle.ClosestPointTo(circle.Center);
        return circle.Contains(closestPoint);
    }
}
