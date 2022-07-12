using System.Drawing;
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
        var entities = entityManager.GetEntitiesMatchingAll(typeof(Collidable), typeof(Transform))
            .Select<Entity, (Entity entity, Transform transform, Collidable collidable)>(e =>
                (e, e.Get<Transform>(), e.Get<Collidable>()))
            .ToList();

        foreach (var (entity, transform, collidable) in entities.Where(e =>
                     e.collidable.CollisionType == CollisionType.Ball)) {
            foreach (var other in entities.Where(other => other.entity != entity)) {
                if (other.transform.Intersects(transform)) {
                    var penetrationVector = transform.RectangleF.CalculatePenetrationVector(other.transform.RectangleF);
                    // transform.Position -= penetrationVector;
                    debugRenderer.DrawLine(transform.Position + transform.Origin, transform.Position + penetrationVector * 10f, Color.Red);
                    // entity.Get<Movable>().Velocity += penetrationVector / (float)deltaTime.TotalMilliseconds;
                    //Debugger.Break();
                }
            }
        }
    }
}
