using System.Diagnostics;
using System.Numerics;

namespace Genjin.Core.Primitives; 

// Real-Time Collision Detection, Christer Ericson, 2005. Chapter 3.5; A Math and Geometry Primer - Lines, Rays, and Segments. pg 53-54    
/// <summary>
///     A two dimensional ray defined by a starting <see cref="Vector2" /> and a direction <see cref="Vector2" />.
/// </summary>
/// <seealso cref="IEquatable{T}" />
[DebuggerDisplay("{DebugDisplayString,nq}")]
public struct Ray2 : IEquatable<Ray2>
{
    /// <summary>
    ///     The starting <see cref="Vector2" /> of this <see cref="Ray2" />.
    /// </summary>
    public Vector2 Position;

    /// <summary>
    ///     The direction <see cref="Vector2" /> of this <see cref="Ray2" />.
    /// </summary>
    public Vector2 Direction;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Ray2" /> structure from the specified position and direction.
    /// </summary>
    /// <param name="position">The starting point.</param>
    /// <param name="direction">The direction vector.</param>
    public Ray2(Vector2 position, Vector2 direction)
    {
        Position = position;
        Direction = direction;
    }

    /// <summary>
    ///     Determines whether this <see cref="Ray2" /> intersects with a specified <see cref="BoundingRectangle" />.
    /// </summary>
    /// <param name="boundingRectangle">The bounding rectangle.</param>
    /// <param name="rayNearDistance">
    ///     When this method returns, contains the distance along the ray to the first intersection
    ///     point with the <paramref name="boundingRectangle" />, if an intersection was found; otherwise,
    ///     <see cref="float.NaN" />.
    ///     This parameter is passed uninitialized.
    /// </param>
    /// <param name="rayFarDistance">
    ///     When this method returns, contains the distance along the ray to the second intersection
    ///     point with the <paramref name="boundingRectangle" />, if an intersection was found; otherwise,
    ///     <see cref="float.NaN" />.
    ///     This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    ///     <c>true</c> if this <see cref="Ray2" /> intersects with <paramref name="boundingRectangle" />; otherwise,
    ///     <c>false</c>.
    /// </returns>
    public bool Intersects(BoundingRectangle boundingRectangle, out float rayNearDistance, out float rayFarDistance)
    {
        // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 5.3; Basic Primitive Tests - Intersecting Lines, Rays, and (Directed Segments). pg 179-181

        var minimum = boundingRectangle.Center - boundingRectangle.HalfExtents;
        var maximum = boundingRectangle.Center + boundingRectangle.HalfExtents;

        // Set to the smallest possible value so the algorithm can find the first hit along the ray
        var minimumDistanceAlongRay = float.MinValue;
        // Set to the maximum possible value so the algorithm can find the last hit along the ray
        var maximumDistanceAlongRay = float.MaxValue;

        // For all relevant slabs which in this case is two.

        // The first, horizontal, slab.
        if (!PrimitivesHelper.IntersectsSlab(Position.X, Direction.X, minimum.X, maximum.X,
                ref minimumDistanceAlongRay,
                ref maximumDistanceAlongRay))
        {
            rayNearDistance = rayFarDistance = float.NaN;
            return false;
        }

        // The second, vertical, slab.
        if (!PrimitivesHelper.IntersectsSlab(Position.Y, Direction.Y, minimum.Y, maximum.Y,
                ref minimumDistanceAlongRay,
                ref maximumDistanceAlongRay))
        {
            rayNearDistance = rayFarDistance = float.NaN;
            return false;
        }

        // Ray intersects the 2 slabs.
        rayNearDistance = minimumDistanceAlongRay < 0 ? 0 : minimumDistanceAlongRay;
        rayFarDistance = maximumDistanceAlongRay;
        return true;
    }

    /// <summary>
    ///     Returns a <see cref="string" /> that represents this <see cref="Ray2" />.
    /// </summary>
    /// <returns>
    ///     A <see cref="string" /> that represents this <see cref="Ray2" />.
    /// </returns>
    public override string ToString()
    {
        return $"Position: {Position}, Direction: {Direction}";
    }

    internal string DebugDisplayString => ToString();

    public bool Equals(Ray2 other) => Position.Equals(other.Position) && Direction.Equals(other.Direction);

    public override bool Equals(object? obj) => obj is Ray2 other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Position, Direction);

    public static bool operator ==(Ray2 left, Ray2 right) {
        return left.Equals(right);
    }

    public static bool operator !=(Ray2 left, Ray2 right) {
        return !(left == right);
    }
}
