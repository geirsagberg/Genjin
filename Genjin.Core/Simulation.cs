using Genjin.Example;

namespace Genjin.Core;

public readonly record struct SimulatedTimeStep(TimeSpan DeltaTime);

public class Simulation : IUpdatable {
    private readonly MessageHub messageHub;
    private readonly ushort maxSkippedUpdates;

    private TimeSpan simulatedTime;

    public bool IsRunning { get; set; } = true;

    public event Action<TimeSpan> OnUpdate = delegate { };

    internal Simulation(MessageHub messageHub, TimeSpan startTime = default, ushort updatesPerSecond = 60,
        ushort maxSkippedUpdates = 5) {
        if (updatesPerSecond == 0) {
            throw new ArgumentOutOfRangeException(nameof(updatesPerSecond), @"Must be non-zero");
        }

        this.messageHub = messageHub;
        this.maxSkippedUpdates = maxSkippedUpdates;
        simulatedTime = startTime;
        UpdateInterval = TimeSpan.FromSeconds(1.0 / updatesPerSecond);
    }

    public float CurrentInterpolation { get; private set; }

    public TimeSpan UpdateInterval { get; }

    public void Update(TimeSpan deltaTime) {
        if (!IsRunning) {
            return;
        }

        var updatesSkipped = 0;
        var targetTime = simulatedTime + deltaTime;
        while (simulatedTime < targetTime && (maxSkippedUpdates == 0 || updatesSkipped < maxSkippedUpdates)) {
            messageHub.Send(new ResetDebugRendererRequest());
            OnUpdate(UpdateInterval);
            simulatedTime += UpdateInterval;
            updatesSkipped++;
        }

        CurrentInterpolation =
            Math.Clamp((float) ((targetTime + UpdateInterval - simulatedTime) / UpdateInterval), 0, 1);
    }
}

public record ResetDebugRendererRequest : IRequest;
