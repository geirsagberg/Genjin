using Genjin.Breakout.Components;
using Genjin.Core;
using Genjin.Core.Entities;

namespace Genjin.Breakout.Systems;

internal class MovableSystem : ISimulationSystem {
    private readonly IEntityManager entityManager;

    public MovableSystem(IEntityManager entityManager) {
        this.entityManager = entityManager;
    }

    public void Update(TimeSpan deltaTime) {
        var entities = entityManager.GetEntitiesMatchingAll(typeof(Movable), typeof(Body));
        foreach (var entity in entities) {
            var movable = entity.GetComponent<Movable>();
            var transform = entity.GetComponent<Body>();
            transform.Position += movable.Velocity * (float) deltaTime.TotalSeconds;
        }
    }
}
