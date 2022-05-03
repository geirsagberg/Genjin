using System.Numerics;

namespace Genjin.Breakout.Components;

record Circle(Vector2 Position, float Radius) : Shape(Position);