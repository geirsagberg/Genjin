using System.Diagnostics;
using System.Numerics;

namespace Genjin.Core.Primitives; 
// Real-Time Collision Detection, Christer Ericson, 2005. Chapter 3.2; A Math and Geometry Primer - Coordinate Systems and Points. pg 35

/// <summary>
///     A two-dimensional point defined by a 2-tuple of real numbers, (x, y).
/// </summary>
/// <remarks>
///     <para>
///         A point is a position in two-dimensional space, the location of which is described in terms of a
///         two-dimensional coordinate system, given by a reference point, called the origin, and two coordinate axes.
///     </para>
///     <para>
///         A common two-dimensional coordinate system is the Cartesian (or rectangular) coordinate system where the
///         coordinate axes, conventionally denoted the X axis and Y axis, are perpindicular to each other. For the
///         three-dimensional rectangular coordinate system, the third axis is called the Z axis.
///     </para>
/// </remarks>
/// <seealso cref="IEquatable{T}" />
[DebuggerDisplay("{DebugDisplayString,nq}")]
public struct Point2F : IEquatable<Point2F>
{
    /// <summary>
    ///     Returns a <see cref="Point2F" /> with <see cref="X" /> and <see cref="Y" /> equal to <c>0.0f</c>.
    /// </summary>
    public static readonly Point2F Zero = new();

    /// <summary>
    ///     Returns a <see cref="Point2F" /> with <see cref="X" /> and <see cref="Y" /> set to not a number.
    /// </summary>
    public static readonly Point2F NaN = new(float.NaN, float.NaN);

    /// <summary>
    ///     The x-coordinate of this <see cref="Point2F" />.
    /// </summary>
    public float X;

    /// <summary>
    ///     The y-coordinate of this <see cref="Point2F" />.
    /// </summary>
    public float Y;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Point2F" /> structure from the specified coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public Point2F(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    ///     Compares two <see cref="Point2F" /> structures. The result specifies
    ///     whether the values of the <see cref="X" /> and <see cref="Y" />
    ///     fields of the two <see cref="Point2F" />
    ///     structures are equal.
    /// </summary>
    /// <param name="first">The first point.</param>
    /// <param name="second">The second point.</param>
    /// <returns>
    ///     <c>true</c> if the <see cref="X" /> and <see cref="Y" />
    ///     fields of the two <see cref="Point2F" />
    ///     structures are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(Point2F first, Point2F second)
    {
        return first.Equals(ref second);
    }

    /// <summary>
    ///     Indicates whether this <see cref="Point2F" /> is equal to another <see cref="Point2F" />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>
    ///     <c>true</c> if this <see cref="Point2F" /> is equal to the <paramref name="point" />; otherwise,
    ///     <c>false</c>.
    /// </returns>
    public bool Equals(Point2F point)
    {
        return Equals(ref point);
    }

    /// <summary>
    ///     Indicates whether this <see cref="Point2F" /> is equal to another <see cref="Point2F" />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>
    ///     <c>true</c> if this <see cref="Point2F" /> is equal to the <paramref name="point" /> parameter; otherwise,
    ///     <c>false</c>.
    /// </returns>
    public bool Equals(ref Point2F point)
    {
        // ReSharper disable CompareOfFloatsByEqualityOperator
        return (point.X == X) && (point.Y == Y);
        // ReSharper restore CompareOfFloatsByEqualityOperator
    }

    /// <summary>
    ///     Returns a value indicating whether this <see cref="Point2F" /> is equal to a specified object.
    /// </summary>
    /// <param name="obj">The object to make the comparison with.</param>
    /// <returns>
    ///     <c>true</c> if this  <see cref="Point2F" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) {
        return obj is Point2F f && Equals(f);
    }

    /// <summary>
    ///     Compares two <see cref="Point2F" /> structures. The result specifies
    ///     whether the values of the <see cref="X" /> or <see cref="Y" />
    ///     fields of the two <see cref="Point2F" />
    ///     structures are unequal.
    /// </summary>
    /// <param name="first">The first point.</param>
    /// <param name="second">The second point.</param>
    /// <returns>
    ///     <c>true</c> if the <see cref="X" /> or <see cref="Y" />
    ///     fields of the two <see cref="Point2F" />
    ///     structures are unequal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(Point2F first, Point2F second)
    {
        return !(first == second);
    }

    /// <summary>
    ///     Calculates the <see cref="Point2F" /> representing the addition of a <see cref="Point2F" /> and a
    ///     <see cref="Vector2" />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="vector">The vector.</param>
    /// <returns>
    ///     The <see cref="Point2F" /> representing the addition of a <see cref="Point2F" /> and a <see cref="Vector2" />.
    /// </returns>
    public static Point2F operator +(Point2F point, Vector2 vector)
    {
        return Add(point, vector);
    }

    /// <summary>
    ///     Calculates the <see cref="Point2F" /> representing the addition of a <see cref="Point2F" /> and a
    ///     <see cref="Vector2" />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="vector">The vector.</param>
    /// <returns>
    ///     The <see cref="Point2F" /> representing the addition of a <see cref="Point2F" /> and a <see cref="Vector2" />.
    /// </returns>
    public static Point2F Add(Point2F point, Vector2 vector)
    {
        Point2F p;
        p.X = point.X + vector.X;
        p.Y = point.Y + vector.Y;
        return p;
    }

    /// <summary>
    ///     Calculates the <see cref="Point2F" /> representing the subtraction of a <see cref="Point2F" /> and a
    ///     <see cref="Vector2" />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="vector">The vector.</param>
    /// <returns>
    ///     The <see cref="Point2F" /> representing the substraction of a <see cref="Point2F" /> and a <see cref="Vector2" />.
    /// </returns>
    public static Point2F operator -(Point2F point, Vector2 vector)
    {
        return Subtract(point, vector);
    }

    /// <summary>
    ///     Calculates the <see cref="Point2F" /> representing the addition of a <see cref="Point2F" /> and a
    ///     <see cref="Vector2" />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="vector">The vector.</param>
    /// <returns>
    ///     The <see cref="Point2F" /> representing the substraction of a <see cref="Point2F" /> and a <see cref="Vector2" />.
    /// </returns>
    public static Point2F Subtract(Point2F point, Vector2 vector)
    {
        Point2F p;
        p.X = point.X - vector.X;
        p.Y = point.Y - vector.Y;
        return p;
    }

    /// <summary>
    ///     Calculates the <see cref="Vector2" /> representing the displacement of two <see cref="Point2F" /> structures.
    /// </summary>
    /// <param name="point2">The second point.</param>
    /// <param name="point1">The first point.</param>
    /// <returns>
    ///     The <see cref="Vector2" /> representing the displacement of two <see cref="Point2F" /> structures.
    /// </returns>
    public static Vector2 operator -(Point2F point1, Point2F point2)
    {
        return Displacement(point1, point2);
    }

    /// <summary>
    ///     Calculates the <see cref="Vector2" /> representing the displacement of two <see cref="Point2F" /> structures.
    /// </summary>
    /// <param name="point2">The second point.</param>
    /// <param name="point1">The first point.</param>
    /// <returns>
    ///     The <see cref="Vector2" /> representing the displacement of two <see cref="Point2F" /> structures.
    /// </returns>
    public static Vector2 Displacement(Point2F point2, Point2F point1)
    {
        Vector2 vector;
        vector.X = point2.X - point1.X;
        vector.Y = point2.Y - point1.Y;
        return vector;
    }

    /// <summary>
    ///     Translates a <see cref='Point2F' /> by a given <see cref='Size2F' />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="size">The size.</param>
    /// <returns>
    ///     The result of the operator.
    /// </returns>
    public static Point2F operator +(Point2F point, Size2F size)
    {
        return Add(point, size);
    }

    /// <summary>
    ///     Translates a <see cref='Point2F' /> by a given <see cref='Size2F' />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="size">The size.</param>
    /// <returns>
    ///     The result of the operator.
    /// </returns>
    public static Point2F Add(Point2F point, Size2F size)
    {
        return new Point2F(point.X + size.Width, point.Y + size.Height);
    }

    /// <summary>
    ///     Translates a <see cref='Point2F' /> by the negative of a given <see cref='Size2F' />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="size">The size.</param>
    /// <returns>
    ///     The result of the operator.
    /// </returns>
    public static Point2F operator -(Point2F point, Size2F size)
    {
        return Subtract(point, size);
    }

    /// <summary>
    ///     Translates a <see cref='Point2F' /> by the negative of a given <see cref='Size2F' /> .
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="size">The size.</param>
    /// <returns>
    ///     The result of the operator.
    /// </returns>
    public static Point2F Subtract(Point2F point, Size2F size)
    {
        return new Point2F(point.X - size.Width, point.Y - size.Height);
    }

    /// <summary>
    ///     Returns a hash code of this <see cref="Point2F" /> suitable for use in hashing algorithms and data
    ///     structures like a hash table.
    /// </summary>
    /// <returns>
    ///     A hash code of this <see cref="Point2F" />.
    /// </returns>
    public override int GetHashCode()
    {
        unchecked
        {
            return (X.GetHashCode()*397) ^ Y.GetHashCode();
        }
    }

    /// <summary>
    ///     Calculates the <see cref="Point2F" /> that contains the minimal coordinate values from two <see cref="Point2F" />
    ///     structures.
    ///     structures.
    /// </summary>
    /// <param name="first">The first point.</param>
    /// <param name="second">The second point.</param>
    /// <returns>
    ///     The the <see cref="Point2F" /> that contains the minimal coordinate values from two <see cref="Point2F" />
    ///     structures.
    /// </returns>
    public static Point2F Minimum(Point2F first, Point2F second)
    {
        return new Point2F(first.X < second.X ? first.X : second.X,
            first.Y < second.Y ? first.Y : second.Y);
    }

    /// <summary>
    ///     Calculates the <see cref="Point2F" /> that contains the maximal coordinate values from two <see cref="Point2F" />
    ///     structures.
    ///     structures.
    /// </summary>
    /// <param name="first">The first point.</param>
    /// <param name="second">The second point.</param>
    /// <returns>
    ///     The the <see cref="Point2F" /> that contains the maximal coordinate values from two <see cref="Point2F" />
    ///     structures.
    /// </returns>
    public static Point2F Maximum(Point2F first, Point2F second)
    {
        return new Point2F(first.X > second.X ? first.X : second.X,
            first.Y > second.Y ? first.Y : second.Y);
    }

    /// <summary>
    ///     Performs an implicit conversion from a <see cref="Point2F" /> to a position <see cref="Vector2" />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>
    ///     The resulting <see cref="Vector2" />.
    /// </returns>
    public static implicit operator Vector2(Point2F point)
    {
        return new Vector2(point.X, point.Y);
    }

    /// <summary>
    ///     Performs an implicit conversion from a <see cref="Vector2" /> to a position <see cref="Point2F" />.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>
    ///     The resulting <see cref="Point2F" />.
    /// </returns>
    public static implicit operator Point2F(Vector2 vector)
    {
        return new Point2F(vector.X, vector.Y);
    }

    /// <summary>
    ///     Returns a <see cref="string" /> that represents this <see cref="Point2F" />.
    /// </summary>
    /// <returns>
    ///     A <see cref="string" /> that represents this <see cref="Point2F" />.
    /// </returns>
    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    internal string DebugDisplayString => ToString();
}
