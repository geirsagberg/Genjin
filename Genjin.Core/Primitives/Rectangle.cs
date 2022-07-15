using System.Numerics;
using System.Runtime.CompilerServices;

namespace Genjin.Core.Primitives;

public record Rectangle(Vector2 Position, float Width, float Height) : IShape {
    public Rectangle(Vector2 position, Size size) : this(position, size.Width, size.Height) { }
    public Vector2 TopLeft => Position;
    public Vector2 BottomRight => Position + Size;
    public float Top => Position.Y;
    public float Bottom => Position.Y + Height;
    public float Left => Position.X;
    public float Right => Position.X + Width;
    public float Width { get; set; } = Width;
    public float Height { get; set; } = Height;

    public Size Size => new(Width, Height);
    public Box Bounds => new(Position, Size);

    public Vector2 Center => Position + (Size / 2f);
    public Vector2 Position { get; set; } = Position;

    public static implicit operator Box(Rectangle rect) => new(rect.Position, rect.Size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 ClosestPointTo(in Vector2 circlePosition) =>
        PrimitivesHelper.ClosestPointToPointFromRectangle(TopLeft, BottomRight,
            circlePosition);
}
