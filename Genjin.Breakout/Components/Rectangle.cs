using System.Numerics;

namespace Genjin.Breakout.Components;

record Rectangle(Vector2 Position, float Width, float Height) : Shape(Position);