namespace Genjin.Breakout.Components;

internal enum CollisionType {
    Wall,
    Ball,
    Brick
}

internal record Collidable(CollisionType CollisionType);
