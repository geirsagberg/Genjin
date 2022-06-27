using Genjin.Breakout.Components;
using Genjin.Core;
using Genjin.Core.Entities;

namespace Genjin.Breakout.Systems;

internal class MovableSystem : ISimulationSystem {
    private readonly IEntityManager entityManager;
    private readonly SharedState sharedState;

    public MovableSystem(IEntityManager entityManager, SharedState sharedState) {
        this.entityManager = entityManager;
        this.sharedState = sharedState;
    }

    public void Update(TimeSpan deltaTime) {
        var entities = entityManager.GetEntitiesMatchingAll(typeof(Movable), typeof(Transform));
        foreach (var entity in entities) {
            var movable = entity.Get<Movable>();
            var transform = entity.Get<Transform>();
            transform.Position += movable.Velocity * (float) deltaTime.TotalSeconds;

            if (entity.TryGet<Collidable>() is { } collidable) {
                if (transform.Position.X < 0) {
                    if (collidable.CollisionResponse == CollisionResponse.Stop) {
                        transform.Position = transform.Position with { X = 0 };
                    } else if (collidable.CollisionResponse == CollisionResponse.Bounce) {
                        movable.Velocity = movable.Velocity with { X = -movable.Velocity.X };
                    }
                } else if (transform.Position.X + transform.Size.Width > sharedState.GameSize.Width) {
                    if (collidable.CollisionResponse == CollisionResponse.Stop) {
                        transform.Position = transform.Position with {
                            X = sharedState.GameSize.Width - transform.Size.Width
                        };
                    } else if (collidable.CollisionResponse == CollisionResponse.Bounce) {
                        movable.Velocity = movable.Velocity with { X = -movable.Velocity.X };
                    }
                } else if (transform.Position.Y < 0) {
                    if (collidable.CollisionResponse == CollisionResponse.Stop) {
                        transform.Position = transform.Position with { Y = 0 };
                    } else if (collidable.CollisionResponse == CollisionResponse.Bounce) {
                        movable.Velocity = movable.Velocity with { Y = -movable.Velocity.Y };
                    }
                } else if (transform.Position.Y + transform.Size.Height > sharedState.GameSize.Height) {
                    if (collidable.CollisionResponse == CollisionResponse.Stop) {
                        transform.Position = transform.Position with {
                            Y = sharedState.GameSize.Height - transform.Size.Height
                        };
                    } else if (collidable.CollisionResponse == CollisionResponse.Bounce) {
                        movable.Velocity = movable.Velocity with { Y = -movable.Velocity.Y };
                    }
                }
            }
        }
    }
}
