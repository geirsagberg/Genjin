using Genjin.Core;
using Genjin.Core.Entities;
using Genjin.Core.Extensions;

namespace Genjin.Breakout.Systems; 

public class SpeedSystem : ISimulationSystem {
    private readonly SharedState sharedState;
    private readonly Provide<InputState<GameKeys>> getGameKeys;

    public SpeedSystem(SharedState sharedState, Provide<InputState<GameKeys>> getGameKeys) {
        this.sharedState = sharedState;
        this.getGameKeys = getGameKeys;
    }
    
    public void Update(TimeSpan deltaTime) {
        var gameKeys = getGameKeys();

        var elapsedSeconds = deltaTime.TotalSeconds.ToFloat();
        
        if (gameKeys.IsKeyDown(GameKeys.IncreaseSpeed)) {
            sharedState.GameSpeed += elapsedSeconds;
        }

        if (gameKeys.IsKeyDown(GameKeys.DecreaseSpeed)) {
            sharedState.GameSpeed -= elapsedSeconds;
        }

        if (gameKeys.IsKeyDown(GameKeys.ResetSpeed)) {
            sharedState.GameSpeed = 1f;
        }
    }
}
