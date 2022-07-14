using System.Numerics;
using System.Runtime.CompilerServices;
using Genjin.Core.Primitives;
using RectangleF = Genjin.Core.Primitives.RectangleF;

namespace Genjin.Core.Extensions;

public static class GeometryExtensions {
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Transform(this Matrix3x2 matrix, Vector2 vector) =>
        new(vector.X * matrix.M11 + vector.Y * matrix.M21 + matrix.M31,
            vector.X * matrix.M12 + vector.Y * matrix.M22 + matrix.M32);

    public static Vector2 Scale(this Vector2 vector2, float scale) => new(vector2.X * scale, vector2.Y * scale);

    public static Vector2 NormalizedOrZero(this Vector2 vector2) =>
        vector2 == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(vector2);

    public static Vector2 WidthVector(this Size2F size) => new(size.Width, 0);
    public static Vector2 HeightVector(this Size2F size) => new(0, size.Height);

    public static Vector2 WidthVector(this RectangleF rectangle) => new(rectangle.Width, 0);
    public static Vector2 HeightVector(this RectangleF rectangle) => new(0, rectangle.Height);

    public static bool IsBetween(this float f, float a, float b) => f >= (a < b ? a : b) && f <= (a < b ? b : a);

    public static bool Contains(this RectangleF first, RectangleF second) =>
        first.Contains(second.TopLeft) && first.Contains(second.BottomRight);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 SubtractLength(this Vector2 vector, float length) =>
        vector.NormalizedOrZero() * (vector.Length() - length);

    public static RectangleF Expand(this RectangleF rectangle, Vector2 direction) {
        if (direction == Vector2.Zero) return rectangle;
        var translated = rectangle;
        translated.Position += direction;
        return rectangle.Union(translated);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CircleF Inflate(this CircleF circle, float addRadius) =>
        new(circle.Center, circle.Radius + addRadius);

    public static void Wrap(this Body body, RectangleF levelSize) {
        if (body.Position.X < 0) {
            body.Position += levelSize.WidthVector();
        } else if (body.Position.X >= levelSize.Width) {
            body.Position -= levelSize.WidthVector();
        }

        if (body.Position.Y < 0) {
            body.Position += levelSize.HeightVector();
        } else if (body.Position.Y >= levelSize.Height) {
            body.Position -= levelSize.HeightVector();
        }
    }

    /// <summary>
    ///     Calculate a's penetration into b
    /// </summary>
    /// <param name="a">The penetrating shape.</param>
    /// <param name="b">The shape being penetrated.</param>
    /// <returns>The distance vector from the edge of b to a's Position</returns>
    public static Vector2 CalculatePenetrationVector<T1, T2>(this T1 a, T2 b) where T1 : IShapeF where T2 : IShapeF {
        if (!a.Intersects(b)) return Vector2.Zero;
        var penetrationVector = a switch {
            RectangleF rectA when b is RectangleF rectB => PenetrationVector(rectA, rectB),
            CircleF circA when b is CircleF circB => PenetrationVector(circA, circB),
            CircleF circA when b is RectangleF rectB => PenetrationVector(circA, rectB),
            RectangleF rectA when b is CircleF circB => PenetrationVector(rectA, circB),
            _ => throw new NotSupportedException("Shapes must be either a CircleF or RectangleF")
        };
        return penetrationVector;
    }

    private static Vector2 PenetrationVector(RectangleF rect1, RectangleF rect2) {
        var intersectingRectangle = RectangleF.Intersection(rect1, rect2);
        if (intersectingRectangle.IsEmpty) return Vector2.Zero;

        Vector2 penetration;
        if (intersectingRectangle.Width < intersectingRectangle.Height) {
            var d = rect1.Center.X < rect2.Center.X
                ? intersectingRectangle.Width
                : -intersectingRectangle.Width;
            penetration = new Vector2(d, 0);
        } else {
            var d = rect1.Center.Y < rect2.Center.Y
                ? intersectingRectangle.Height
                : -intersectingRectangle.Height;
            penetration = new Vector2(0, d);
        }

        return penetration;
    }

    private static Vector2 PenetrationVector(CircleF circ1, CircleF circ2) {
        if (!circ1.Intersects(circ2)) {
            return Vector2.Zero;
        }

        var displacement = circ2.Center - circ1.Center;

        Vector2 desiredDisplacement;
        if (displacement != Vector2.Zero) {
            desiredDisplacement = displacement.NormalizedCopy() * (circ1.Radius + circ2.Radius);
        } else {
            desiredDisplacement = -Vector2.UnitY * (circ1.Radius + circ2.Radius);
        }

        var penetration = displacement - desiredDisplacement;
        return penetration;
    }

    private static Vector2 PenetrationVector(CircleF circ, RectangleF rect) {
        var collisionPoint = rect.ClosestPointTo(circ.Center);
        var cToCollPoint = collisionPoint - circ.Center;

        if (rect.Contains(circ.Center) || cToCollPoint.Equals(Vector2.Zero)) {
            var displacement = rect.Center - circ.Center;

            Vector2 desiredDisplacement;
            if (displacement != Vector2.Zero) {
                // Calculate penetration as only in X or Y direction.
                // Whichever is lower.
                var dispx = (displacement with { Y = 0 }).NormalizedCopy();
                var dispy = (displacement with { X = 0 }).NormalizedCopy();

                dispx *= circ.Radius + (rect.Width / 2);
                dispy *= circ.Radius + (rect.Height / 2);

                if (dispx.LengthSquared() < dispy.LengthSquared()) {
                    desiredDisplacement = dispx;
                    displacement.Y = 0;
                } else {
                    desiredDisplacement = dispy;
                    displacement.X = 0;
                }
            } else {
                desiredDisplacement = -Vector2.UnitY * (circ.Radius + (rect.Height / 2));
            }

            var penetration = displacement - desiredDisplacement;
            return penetration;
        } else {
            var penetration = (circ.Radius * cToCollPoint.NormalizedCopy()) - cToCollPoint;
            return penetration;
        }
    }

    private static Vector2 PenetrationVector(RectangleF rect, CircleF circ) {
        return -PenetrationVector(circ, rect);
    }
}

