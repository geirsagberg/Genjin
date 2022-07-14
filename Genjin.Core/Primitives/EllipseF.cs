using System.Numerics;
using System.Runtime.Serialization;

namespace Genjin.Core.Primitives;

[DataContract]
public struct EllipseF : IEquatable<EllipseF>, IShapeF {
    [DataMember]
    public Vector2 Center { get; set; }

    [DataMember]
    public float RadiusX { get; set; }

    [DataMember]
    public float RadiusY { get; set; }

    public Vector2 Position {
        get => Center;
        set => Center = value;
    }

    public EllipseF(Vector2 center, float radiusX, float radiusY) {
        Center = center;
        RadiusX = radiusX;
        RadiusY = radiusY;
    }

    public float Left => Center.X - RadiusX;
    public float Top => Center.Y - RadiusY;
    public float Right => Center.X + RadiusX;
    public float Bottom => Center.Y + RadiusY;

    public RectangleF BoundingRectangle {
        get {
            var minX = Left;
            var minY = Top;
            var maxX = Right;
            var maxY = Bottom;
            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }
    }

    public bool Contains(float x, float y) {
        var xCalc = (float) (Math.Pow(x - Center.X, 2) / Math.Pow(RadiusX, 2));
        var yCalc = (float) (Math.Pow(y - Center.Y, 2) / Math.Pow(RadiusY, 2));

        return xCalc + yCalc <= 1;
    }

    public bool Contains(Vector2 point) => Contains(point.X, point.Y);

    public override string ToString() => $"Centre: {Center}, RadiusX: {RadiusX}, RadiusY: {RadiusY}";

    public bool Equals(EllipseF other) =>
        Center.Equals(other.Center) && RadiusX.Equals(other.RadiusX) && RadiusY.Equals(other.RadiusY);

    public override bool Equals(object? obj) => obj is EllipseF other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Center, RadiusX, RadiusY);

    public static bool operator ==(EllipseF left, EllipseF right) => left.Equals(right);

    public static bool operator !=(EllipseF left, EllipseF right) => !(left == right);
}
