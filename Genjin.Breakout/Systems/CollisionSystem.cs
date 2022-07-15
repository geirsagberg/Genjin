using System.Drawing;
using System.Numerics;
using Genjin.Breakout.Components;
using Genjin.Core;
using Genjin.Core.Entities;
using Genjin.Core.Extensions;
using Genjin.Core.Primitives;

namespace Genjin.Breakout.Systems;

public class CollisionSystem : ISimulationSystem {
    private readonly IDebugRenderer debugRenderer;
    private readonly IEntityManager entityManager;

    public CollisionSystem(IEntityManager entityManager, IDebugRenderer debugRenderer) {
        this.entityManager = entityManager;
        this.debugRenderer = debugRenderer;
    }

    public void Update(TimeSpan deltaTime) {
        var entities = entityManager.GetEntitiesMatchingAll(typeof(Collidable), typeof(Body))
            .ToList();

        for (var i = 0; i < entities.Count; i++) {
            var entity = entities[i];
            var collidable = entity.GetComponent<Collidable>();
            var body = entity.GetComponent<Body>();
            var shape = body.Shape;
            for (var j = i + 1; j < entities.Count; j++) {
                var otherEntity = entities[j];
                var otherBody = otherEntity.GetComponent<Body>();
                var otherShape = otherBody.Shape;
                if (shape.Intersects(otherShape)) {
                    var penetrationVector = shape.CalculatePenetrationVector(otherShape);

                    if (penetrationVector != Vector2.Zero) {
                        var otherCollidable = otherEntity.GetComponent<Collidable>();
                        HandleCollision(entity, collidable, body, otherEntity, otherCollidable, otherBody, penetrationVector);
                        HandleCollision(otherEntity, otherCollidable, otherBody, entity, collidable, body, -penetrationVector);
                    }
                }
            }
        }
    }

    private void HandleCollision(Entity entity, Collidable collidable, Body body, Entity otherEntity,
        Collidable otherCollidable, Body otherBody, Vector2 penetrationVector) {
        debugRenderer.DrawVector(body.Center, penetrationVector * 10f, Color.Red);

        if (collidable.CollisionType == CollisionType.Ball) {
            var movable = entity.GetComponent<Movable>();
            var normal = penetrationVector.NormalizedCopy();
            body.Position -= penetrationVector;
            movable.Velocity = movable.Velocity.Reflect(normal);

            if (otherCollidable.CollisionType == CollisionType.Paddle) {
                var paddle = otherEntity.GetComponent<Movable>();
                movable.Velocity += paddle.Velocity;
            }
        }
    }
}
