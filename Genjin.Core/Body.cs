using System.Numerics;
using Genjin.Core.Primitives;

namespace Genjin.Core;

public record Body {
    public Body(Vector2 position, Size2F size) : this(position, new RectangleF(),
        new Vector2(size.Width / 2f, size.Height / 2f)) {
    }

    public Body(Vector2 position, IShapeF shape, Vector2 origin, float rotation = default, Vector2 scale = default) {
        Shape = shape;
        Origin = origin;
        Scale = scale;
        Rotation = rotation;
        Position = position;
    }
    
    /**
     * M11 = x scale, rotation
     * M12 = rotation
     * M21 = rotation
     * M22 = y scale, rotation
     * M31 = x translation
     * M32 = y translation
     */
    public Matrix3x2 Transform {
        get {
            var scale = new Vector2(Scale.X, Scale.Y);
            var rotation = Rotation;
            var position = Position;
            var origin = Origin;
            return Matrix3x2.CreateScale(scale) * Matrix3x2.CreateRotation(rotation) * Matrix3x2.CreateTranslation(position - origin);
        }
    }

    public IShapeF Shape { get; set; }

    /// Original size
    public Size2F Size => Shape.Size;

    public Vector2 Origin { get; set; }

    public Vector2 Scale { get; set; }

    public float Rotation { get; set; }

    public Vector2 Position { get; set; }

    public RectangleF RectangleF => new(Position.X, Position.Y, Size.Width, Size.Height);

    public bool Intersects(Body body) => RectangleF.Intersects(body.RectangleF);
}
