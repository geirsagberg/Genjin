using System.Numerics;
using System.Runtime.CompilerServices;
using Genjin.Core.Extensions;

namespace Genjin.Core.Primitives;

public record Circle(Vector2 Position, float Radius) : IShape {
    public float Diameter => Radius * 2;

    public float Radius { get; set; } = Radius;
    public Size Size => new(Diameter, Diameter);
    public Box Bounds => Box.FromCenter(Position, Size);

    public Vector2 Center => Position;
    public Vector2 Position { get; set; } = Position;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(Vector2 point) => Vector2.DistanceSquared(Center, point) < Radius.Squared();
}
