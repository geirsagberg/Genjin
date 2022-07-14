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
public struct Size2F : IEquatable<Size2F> {
    /// <summary>
    ///     Returns a <see cref="Size2F" /> with <see cref="Width" /> and <see cref="Height" /> equal to <c>0.0f</c>.
    /// </summary>
    public static readonly Size2F Empty = new();

    /// <summary>
    ///     The horizontal component of this <see cref="Size2F" />.
    /// </summary>
    public float Width;

    /// <summary>
    ///     The vertical component of this <see cref="Size2F" />.
    /// </summary>
    public float Height;

    /// <summary>
    ///     Gets a value that indicates whether this <see cref="Size2F" /> is empty.
    /// </summary>
    // ReSharper disable CompareOfFloatsByEqualityOperator
    public bool IsEmpty => Width == 0 && Height == 0;

    // ReSharper restore CompareOfFloatsByEqualityOperator

    /// <summary>
    ///     Initializes a new instance of the <see cref="Size2F" /> structure from the specified dimensions.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public Size2F(float width, float height) {
        Width = width;
        Height = height;
    }

    /// <summary>
    ///     Compares two <see cref="Size2F" /> structures. The result specifies
    ///     whether the values of the <see cref="Width" /> and <see cref="Height" />
    ///     fields of the two <see cref="Vector2" /> structures are equal.
    /// </summary>
    /// <param name="first">The first size.</param>
    /// <param name="second">The second size.</param>
    /// <returns>
    ///     <c>true</c> if the <see cref="Width" /> and <see cref="Height" />
    ///     fields of the two <see cref="Vector2" /> structures are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(Size2F first, Size2F second) {
        return first.Equals(ref second);
    }

    /// <summary>
    ///     Indicates whether this <see cref="Size2F" /> is equal to another <see cref="Size2F" />.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>
    ///     <c>true</c> if this <see cref="Vector2" /> is equal to the <paramref name="size" /> parameter; otherwise,
    ///     <c>false</c>.
    /// </returns>
    public bool Equals(Size2F size) {
        return Equals(ref size);
    }

    /// <summary>
    ///     Indicates whether this <see cref="Size2F" /> is equal to another <see cref="Size2F" />.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>
    ///     <c>true</c> if this <see cref="Vector2" /> is equal to the <paramref name="size" />; otherwise,
    ///     <c>false</c>.
    /// </returns>
    public bool Equals(ref Size2F size) {
        // ReSharper disable CompareOfFloatsByEqualityOperator
        return Width == size.Width && Height == size.Height;
        // ReSharper restore CompareOfFloatsByEqualityOperator
    }

    /// <summary>
    ///     Returns a value indicating whether this <see cref="Size2F" /> is equal to a specified object.
    /// </summary>
    /// <param name="obj">The object to make the comparison with.</param>
    /// <returns>
    ///     <c>true</c> if this  <see cref="Size2F" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) {
        if (obj is Size2F size2)
            return Equals(size2);
        return false;
    }

    /// <summary>
    ///     Compares two <see cref="Size2F" /> structures. The result specifies
    ///     whether the values of the <see cref="Width" /> or <see cref="Height" />
    ///     fields of the two <see cref="Size2F" /> structures are unequal.
    /// </summary>
    /// <param name="first">The first size.</param>
    /// <param name="second">The second size.</param>
    /// <returns>
    ///     <c>true</c> if the <see cref="Width" /> or <see cref="Height" />
    ///     fields of the two <see cref="Size2F" /> structures are unequal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(Size2F first, Size2F second) {
        return !(first == second);
    }

    /// <summary>
    ///     Calculates the <see cref="Size2F" /> representing the vector addition of two <see cref="Size2F" /> structures as if
    ///     they
    ///     were <see cref="Vector2" /> structures.
    /// </summary>
    /// <param name="first">The first size.</param>
    /// <param name="second">The second size.</param>
    /// <returns>
    ///     The <see cref="Size2F" /> representing the vector addition of two <see cref="Size2F" /> structures as if they
    ///     were <see cref="Vector2" /> structures.
    /// </returns>
    public static Size2F operator +(Size2F first, Size2F second) {
        return Add(first, second);
    }

    /// <summary>
    ///     Calculates the <see cref="Size2F" /> representing the vector addition of two <see cref="Size2F" /> structures.
    /// </summary>
    /// <param name="first">The first size.</param>
    /// <param name="second">The second size.</param>
    /// <returns>
    ///     The <see cref="Size2F" /> representing the vector addition of two <see cref="Size2F" /> structures.
    /// </returns>
    public static Size2F Add(Size2F first, Size2F second) {
        Size2F size;
        size.Width = first.Width + second.Width;
        size.Height = first.Height + second.Height;
        return size;
    }

    /// <summary>
    ///     Calculates the <see cref="Size2F" /> representing the vector subtraction of two <see cref="Size2F" /> structures.
    /// </summary>
    /// <param name="first">The first size.</param>
    /// <param name="second">The second size.</param>
    /// <returns>
    ///     The <see cref="Size2F" /> representing the vector subtraction of two <see cref="Size2F" /> structures.
    /// </returns>
    public static Size2F operator -(Size2F first, Size2F second) {
        return Subtract(first, second);
    }

    public static Size2F operator /(Size2F size, float value) {
        return new Size2F(size.Width / value, size.Height / value);
    }

    public static Size2F operator *(Size2F size, float value) {
        return new Size2F(size.Width * value, size.Height * value);
    }

    /// <summary>
    ///     Calculates the <see cref="Size2F" /> representing the vector subtraction of two <see cref="Size2F" /> structures.
    /// </summary>
    /// <param name="first">The first size.</param>
    /// <param name="second">The second size.</param>
    /// <returns>
    ///     The <see cref="Size2F" /> representing the vector subtraction of two <see cref="Size2F" /> structures.
    /// </returns>
    public static Size2F Subtract(Size2F first, Size2F second) {
        Size2F size;
        size.Width = first.Width - second.Width;
        size.Height = first.Height - second.Height;
        return size;
    }

    /// <summary>
    ///     Returns a hash code of this <see cref="Size2F" /> suitable for use in hashing algorithms and data
    ///     structures like a hash table.
    /// </summary>
    /// <returns>
    ///     A hash code of this <see cref="Vector2" />.
    /// </returns>
    public override int GetHashCode() {
        unchecked {
            return (Width.GetHashCode() * 397) ^ Height.GetHashCode();
        }
    }

    /// <summary>
    ///     Performs an implicit conversion from a <see cref="Vector2" /> to a <see cref="Size2F" />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>
    ///     The resulting <see cref="Size2F" />.
    /// </returns>
    public static implicit operator Size2F(Vector2 point) {
        return new Size2F(point.X, point.Y);
    }

    /// <summary>
    ///     Performs an implicit conversion from a <see cref="Size2F" /> to a <see cref="Vector2" />.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>
    ///     The resulting <see cref="Vector2" />.
    /// </returns>
    public static implicit operator Vector2(Size2F size) {
        return new Vector2(size.Width, size.Height);
    }

    /// <summary>
    ///     Returns a <see cref="string" /> that represents this <see cref="Size2F" />.
    /// </summary>
    /// <returns>
    ///     A <see cref="string" /> that represents this <see cref="Size2F" />.
    /// </returns>
    public override string ToString() {
        return $"Width: {Width}, Height: {Height}";
    }

    internal string DebugDisplayString => ToString();
}
