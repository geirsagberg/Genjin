using System.Numerics;
using Genjin.Core.Primitives;

namespace Genjin.Breakout.Components;

public record Body(IShape Shape) {
    public Vector2 Position {
        get => Shape.Position;
        set => Shape.Position = value;
    }

    public Vector2 Center => Shape.Center;
    public Size Size => Shape.Size;
    public Box Bounds => Shape.Bounds;
}
