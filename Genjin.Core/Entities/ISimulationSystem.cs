namespace Genjin.Core.Entities;

public interface ISimulationSystem : ISystem {
    ValueTask Update(TimeSpan deltaTime);
}
