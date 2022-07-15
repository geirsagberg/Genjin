// MIT-licensed (https://github.com/craftworkgames/MonoGame.Extended)

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
public readonly record struct Size(float Width = 0, float Height = 0) {
    public static readonly Size Empty = default;

    public bool IsEmpty => this == Empty;

    public static Size operator /(Size size, float value) {
        return new Size(size.Width / value, size.Height / value);
    }

    public static Size operator *(Size size, float value) {
        return new Size(size.Width * value, size.Height * value);
    }
    
    public static implicit operator Size(Vector2 vector) {
        return new Size(vector.X, vector.Y);
    }

    public static implicit operator Vector2(Size size) {
        return new Vector2(size.Width, size.Height);
    }
}
