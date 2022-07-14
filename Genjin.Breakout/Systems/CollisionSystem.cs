using System.Drawing;
using System.Numerics;
using Genjin.Breakout.Components;
using Genjin.Core;
using Genjin.Core.Entities;
using Genjin.Core.Extensions;

namespace Genjin.Breakout.Systems;

public class CollisionSystem : ISimulationSystem {
    private readonly IEntityManager entityManager;
    private readonly IDebugRenderer debugRenderer;

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
            if (collidable.CollisionType == CollisionType.Ball) {
                var transform = entity.GetComponent<Body>();
                for (var j = i + 1; j < entities.Count; j++) {
                    var otherEntity = entities[j];
                    var otherTransform = otherEntity.GetComponent<Body>();
                    if (transform.Intersects(otherTransform)) {
                        var penetrationVector =
                            transform.RectangleF.CalculatePenetrationVector(otherTransform.RectangleF);

                        if (penetrationVector != Vector2.Zero) {
                            debugRenderer.DrawVector(transform.Position + transform.Origin,  penetrationVector * 10f, Color.Red);
                            var otherCollidable = otherEntity.GetComponent<Collidable>();
                            
                            HandleCollision(entity, collidable, otherEntity, otherCollidable, penetrationVector);
                        }
                    }
                }   
            }
        }
    }

    private void HandleCollision(Entity entity, Collidable collidable, Entity otherEntity, Collidable otherCollidable, Vector2 penetrationVector) {
        if (collidable.CollisionType == CollisionType.Ball) {
            
        }
    }
}
