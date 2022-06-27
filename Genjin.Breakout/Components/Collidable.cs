namespace Genjin.Breakout.Components;

internal enum CollisionResponse {
    Stop,
    Bounce
}

internal record Collidable(CollisionResponse CollisionResponse);
