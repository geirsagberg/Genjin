using CommunityToolkit.Mvvm.Messaging;
using Genjin.Core.Messages;
using Genjin.Example;

namespace Genjin.Core;

public readonly record struct SimulatedTimeStep(TimeSpan DeltaTime);

public record SetGameSpeedRequest(float Speed);

public class Simulation : IUpdatable {
    private readonly MessageHub messageHub;
    private readonly ushort maxSkippedUpdates;

    private TimeSpan simulatedTime;
    private float gameSpeed = 1f;
    private readonly TimeSpan normalizedUpdateInterval;

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
        normalizedUpdateInterval = TimeSpan.FromSeconds(1.0 / updatesPerSecond);
        
        WeakReferenceMessenger.Default.Register<Simulation, SetGameSpeedRequest>(this, (_, request) => {
            gameSpeed = request.Speed;
        });
    }

    public float CurrentInterpolation { get; private set; }

    public TimeSpan UpdateInterval => normalizedUpdateInterval * gameSpeed;

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
