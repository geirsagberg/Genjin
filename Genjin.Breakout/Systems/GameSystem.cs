using System.Numerics;
using Genjin.Breakout.Components;
using Genjin.Core;
using Genjin.Core.Entities;

namespace Genjin.Breakout.Systems;

public class GameSystem : ISimulationSystem {
    private readonly Provide<InputState<GameKeys>> getInputs;
    private readonly IEntityManager entityManager;
    private readonly SharedState sharedState;

    public GameSystem(Provide<InputState<GameKeys>> getInputs, IEntityManager entityManager, SharedState sharedState) {
        this.getInputs = getInputs;
        this.entityManager = entityManager;
        this.sharedState = sharedState;
    }

    public void Update(TimeSpan deltaTime) {
        if (sharedState.GameState == GameState.Initial) {
            if (getInputs().Pressed.Any()) {
                sharedState.GameState = GameState.Playing;
                foreach (var entity in entityManager.GetEntitiesMatchingAll(typeof(Ball), typeof(Movable))) {
                    entity.Get<Movable>().Velocity = new Vector2(1, -1);
                }
            }
        }
    }
}
