using System.Numerics;
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
            if (transform.Position.X < 0) {
                transform.Position = transform.Position with { X = 0 };
            } else if (transform.Position.X + transform.Size.Width > sharedState.GameSize.Width) {
                transform.Position = transform.Position with { X = sharedState.GameSize.Width - transform.Size.Width };
            }
        }
    }
}
