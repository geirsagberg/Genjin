namespace Genjin.Core.Entities;

public interface ISimulationSystem : ISystem {
    void Update(TimeSpan deltaTime);
}
