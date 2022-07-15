using System.Numerics;
using Genjin.Core.Extensions;

namespace Genjin.Core.Primitives;

public static class BodyHelper {
    public static bool Intersects(Circle first, Circle second) {
        var sumRadius = first.Radius + second.Radius;
        return Vector2.DistanceSquared(first.Center, second.Center) < sumRadius.Squared();
    }

    public static bool Intersects(Circle circle, Rectangle rectangle) {
        var closestPoint = rectangle.ClosestPointTo(circle.Position);
        return circle.Contains(closestPoint);
    }

    public static bool Intersects(Rectangle first, Rectangle second) =>
        first.Left <= second.Right && 
        first.Right >= second.Left && 
        first.Top <= second.Bottom &&
        first.Bottom >= second.Top;

    public static bool Intersects(this IShape first, IShape second) => first switch {
        Circle circle => second switch {
            Circle circle2 => Intersects(circle, circle2),
            Rectangle rectangle => Intersects(circle, rectangle),
            _ => false
        },
        Rectangle rectangle => second switch {
            Circle circle => Intersects(circle, rectangle),
            Rectangle rectangle2 => Intersects(rectangle, rectangle2),
            _ => false
        },
        _ => false
    };
}
