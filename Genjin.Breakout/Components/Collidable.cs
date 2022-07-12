namespace Genjin.Breakout.Components;

internal enum CollisionType {
    Wall,
    Ball,
    Brick,
    Paddle
}

internal record Collidable(CollisionType CollisionType);
