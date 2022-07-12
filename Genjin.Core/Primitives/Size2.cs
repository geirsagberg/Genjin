// MIT-licensed (https://github.com/craftworkgames/MonoGame.Extended)

using System.Drawing;
using System.Numerics;

namespace Genjin.Core.Primitives;

/// <summary>
///     A two dimensional size defined by two real numbers, a width and a height.
/// </summary>
/// <remarks>
///     <para>
///         A size is a subspace of two-dimensional space, the area of which is described in terms of a two-dimensional
///         coordinate system, given by a reference point and two coordinate axes.
///     </para>
/// </remarks>
/// <seealso cref="IEquatable{T}" />
public struct Size2 : IEquatable<Size2> {
    /// <summary>
    ///     Returns a <see cref="Size2" /> with <see cref="Width" /> and <see cref="Height" /> equal to <c>0.0f</c>.
    /// </summary>
    public static readonly Size2 Empty = new();

    /// <summary>
    ///     The horizontal component of this <see cref="Size2" />.
    /// </summary>
    public int Width;

    /// <summary>
    ///     The vertical component of this <see cref="Size2" />.
    /// </summary>
    public int Height;

    /// <summary>
    ///     Gets a value that indicates whether this <see cref="Size2" /> is empty.
    /// </summary>
    public bool IsEmpty => Width == 0 && Height == 0;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Size2" /> structure from the specified dimensions.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public Size2(int width, int height) {
        Width = width;
        Height = height;
    }

    /// <summary>
    ///     Compares two <see cref="Size2" /> structures. The result specifies
    ///     whether the values of the <see cref="Width" /> and <see cref="Height" />
    ///     fields of the two <see cref="Point" /> structures are equal.
    /// </summary>
    /// <param name="first">The first size.</param>
    /// <param name="second">The second size.</param>
    /// <returns>
    ///     <c>true</c> if the <see cref="Width" /> and <see cref="Height" />
    ///     fields of the two <see cref="Point" /> structures are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(Size2 first, Size2 second) {
        return first.Equals(ref second);
    }

    /// <summary>
    ///     Indicates whether this <see cref="Size2" /> is equal to another <see cref="Size2" />.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>
    ///     <c>true</c> if this <see cref="Point" /> is equal to the <paramref name="size" /> parameter; otherwise,
    ///     <c>false</c>.
    /// </returns>
    public bool Equals(Size2 size) {
        return Equals(ref size);
    }

    /// <summary>
    ///     Indicates whether this <see cref="Size2" /> is equal to another <see cref="Size2" />.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>
    ///     <c>true</c> if this <see cref="Point" /> is equal to the <paramref name="size" />; otherwise,
    ///     <c>false</c>.
    /// </returns>
    public bool Equals(ref Size2 size) {
        return Width == size.Width && Height == size.Height;
    }

    /// <summary>
    ///     Returns a value indicating whether this <see cref="Size2" /> is equal to a specified object.
    /// </summary>
    /// <param name="obj">The object to make the comparison with.</param>
    /// <returns>
    ///     <c>true</c> if this  <see cref="Size2" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) {
        if (obj is Size2 size)
            return Equals(size);
        return false;
    }

    /// <summary>
    ///     Compares two <see cref="Size2" /> structures. The result specifies
    ///     whether the values of the <see cref="Width" /> or <see cref="Height" />
    ///     fields of the two <see cref="Size2" /> structures are unequal.
    /// </summary>
    /// <param name="first">The first size.</param>
    /// <param name="second">The second size.</param>
    /// <returns>
    ///     <c>true</c> if the <see cref="Width" /> or <see cref="Height" />
    ///     fields of the two <see cref="Size2" /> structures are unequal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(Size2 first, Size2 second) {
        return !(first == second);
    }

    /// <summary>
    ///     Calculates the <see cref="Size2" /> representing the vector addition of two <see cref="Size2" /> structures as if
    ///     they
    ///     were <see cref="Vector2" /> structures.
    /// </summary>
    /// <param name="first">The first size.</param>
    /// <param name="second">The second size.</param>
    /// <returns>
    ///     The <see cref="Size2" /> representing the vector addition of two <see cref="Size2" /> structures as if they
    ///     were <see cref="Vector2" /> structures.
    /// </returns>
    public static Size2 operator +(Size2 first, Size2 second) {
        return Add(first, second);
    }

    /// <summary>
    ///     Calculates the <see cref="Size2" /> representing the vector addition of two <see cref="Size2" /> structures.
    /// </summary>
    /// <param name="first">The first size.</param>
    /// <param name="second">The second size.</param>
    /// <returns>
    ///     The <see cref="Size2" /> representing the vector addition of two <see cref="Size2" /> structures.
    /// </returns>
    public static Size2 Add(Size2 first, Size2 second) {
        Size2 size;
        size.Width = first.Width + second.Width;
        size.Height = first.Height + second.Height;
        return size;
    }

    /// <summary>
    ///     Calculates the <see cref="Size2" /> representing the vector subtraction of two <see cref="Size2" /> structures.
    /// </summary>
    /// <param name="first">The first size.</param>
    /// <param name="second">The second size.</param>
    /// <returns>
    ///     The <see cref="Size2" /> representing the vector subtraction of two <see cref="Size2" /> structures.
    /// </returns>
    public static Size2 operator -(Size2 first, Size2 second) {
        return Subtract(first, second);
    }

    public static Size2 operator /(Size2 size, int value) {
        return new Size2(size.Width / value, size.Height / value);
    }

    public static Size2 operator *(Size2 size, int value) {
        return new Size2(size.Width * value, size.Height * value);
    }

    /// <summary>
    ///     Calculates the <see cref="Size2" /> representing the vector subtraction of two <see cref="Size2" /> structures.
    /// </summary>
    /// <param name="first">The first size.</param>
    /// <param name="second">The second size.</param>
    /// <returns>
    ///     The <see cref="Size2" /> representing the vector subtraction of two <see cref="Size2" /> structures.
    /// </returns>
    public static Size2 Subtract(Size2 first, Size2 second) {
        Size2 size;
        size.Width = first.Width - second.Width;
        size.Height = first.Height - second.Height;
        return size;
    }

    /// <summary>
    ///     Returns a hash code of this <see cref="Size2" /> suitable for use in hashing algorithms and data
    ///     structures like a hash table.
    /// </summary>
    /// <returns>
    ///     A hash code of this <see cref="Point" />.
    /// </returns>
    public override int GetHashCode() {
        unchecked {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return (Width.GetHashCode() * 397) ^ Height.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }
    }

    /// <summary>
    ///     Performs an implicit conversion from a <see cref="Point" /> to a <see cref="Size2" />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>
    ///     The resulting <see cref="Size2" />.
    /// </returns>
    public static implicit operator Size2(Point point) {
        return new Size2(point.X, point.Y);
    }

    /// <summary>
    ///     Performs an implicit conversion from a <see cref="Point" /> to a <see cref="Size2" />.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>
    ///     The resulting <see cref="Point" />.
    /// </returns>
    public static implicit operator Point(Size2 size) {
        return new Point(size.Width, size.Height);
    }

    public static explicit operator Size2(Size2F size) {
        return new Size2((int) size.Width, (int) size.Height);
    }

    /// <summary>
    ///     Returns a <see cref="string" /> that represents this <see cref="Size2" />.
    /// </summary>
    /// <returns>
    ///     A <see cref="string" /> that represents this <see cref="Size2" />.
    /// </returns>
    public override string ToString() {
        return $"Width: {Width}, Height: {Height}";
    }

    internal string DebugDisplayString => ToString();
}
