using Genjin.Breakout.Components;
using Genjin.Core;
using Genjin.Core.Entities;

namespace Genjin.Breakout.Systems;

public class CollisionSystem : ISimulationSystem {
    private readonly IEntityManager entityManager;
    
    

    public CollisionSystem(IEntityManager entityManager) {
        this.entityManager = entityManager;
    }
    
    public void Update(TimeSpan deltaTime) {
        var entities = entityManager.GetEntitiesMatchingAll(typeof(Collidable), typeof(Transform))
            .Select<Entity, (Entity entity, Transform transform, Collidable collidable)>(e => (e, e.Get<Transform>(), e.Get<Collidable>()))
            .ToList();
         
        foreach (var (entity, transform, collidable) in entities.Where(e => e.collidable.CollisionType == CollisionType.Ball)) {
            foreach (var other in entities.Where(other => other.entity == entity)) {
                if (other.transform.IntersectsWith(transform)) {
                    
                }
            }
        }
    }
}
