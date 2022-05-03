using System.Drawing;
using System.Numerics;

namespace Genjin.Example;

/**
 * M11 = x scale, rotation
 * M12 = rotation
 * M21 = rotation
 * M22 = y scale, rotation
 * M31 = x translation
 * M32 = y translation
 * Size = Original size
 */
public readonly record struct Transform(
    Vector2 Position,
    float Rotation,
    Vector2 Scale,
    Vector2 Origin,
    Size Size
) {
    public Transform(Vector2 position, float rotation, Size size) : this(position, rotation, Vector2.One,
        new Vector2(size.Width / 2f, size.Height / 2f),
        size) {
    }

    // public Rectangle Rectangle => new((int)Position.X, (int)Position.Y, (int)(Size.Width * Scale.X),
    //     (int)(Size.Height * Scale.Y));
}
