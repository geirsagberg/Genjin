using Genjin.Core;
using Genjin.Core.Entities;
using Veldrid;

namespace Genjin.Breakout;

public class InputSystem : ISimulationSystem {
    public ValueTask Update(TimeSpan simulatedTime) => throw new NotImplementedException();
}

public interface IRequire<in T> {
    public void Update(T r1);
}

public interface IRequire<in T1, in T2> {
    public void Update(T1 r1, T2 r2);
}

public class Reader<T> {
    public T Value { get; }
}

public class SystemThatRegistersDelegates {
}
