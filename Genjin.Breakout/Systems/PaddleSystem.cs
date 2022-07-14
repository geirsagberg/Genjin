using System.Numerics;
using Genjin.Breakout.Components;
using Genjin.Core;
using Genjin.Core.Entities;

namespace Genjin.Breakout;

internal class PaddleSystem : ISimulationSystem {
    private readonly IEntityManager entityManager;
    private readonly Provide<InputState<GameKeys>> getGameKeys;

    public PaddleSystem(IEntityManager entityManager, Provide<InputState<GameKeys>> getGameKeys) {
        this.entityManager = entityManager;
        this.getGameKeys = getGameKeys;
    }

    public void Update(TimeSpan deltaTime) {
        var gameKeys = getGameKeys();
        var paddles = entityManager.GetEntitiesMatchingAll(typeof(Controllable), typeof(Movable));

        foreach (var paddle in paddles) {
            var velocity = Vector2.Zero;
            if (gameKeys.IsKeyDown(GameKeys.Left)) {
                velocity.X -= 1;
            }

            if (gameKeys.IsKeyDown(GameKeys.Right)) {
                velocity.X += 1;
            }

            paddle.GetComponent<Movable>().Velocity = velocity * 100;
        }
    }
}
