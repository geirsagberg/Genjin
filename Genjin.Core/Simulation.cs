namespace Genjin.Core;

public class Simulation {
    private readonly ushort maxSkippedUpdates;

    private TimeSpan simulatedTime;

    public event Func<TimeSpan, ValueTask> OnUpdate = delegate { return default; };

    internal Simulation(TimeSpan startTime = default, ushort updatesPerSecond = 60,
        ushort maxSkippedUpdates = 5) {
        if (updatesPerSecond == 0) {
            throw new ArgumentOutOfRangeException(nameof(updatesPerSecond), "Must be non-zero");
        }

        this.maxSkippedUpdates = maxSkippedUpdates;
        simulatedTime = startTime;
        UpdateInterval = TimeSpan.FromSeconds(1.0 / updatesPerSecond);
    }

    public float CurrentInterpolation { get; private set; }

    public TimeSpan UpdateInterval { get; }

    public async ValueTask Update(TimeSpan realTime) {
        var updatesSkipped = 0;
        while (simulatedTime < realTime && (maxSkippedUpdates == 0 || updatesSkipped < maxSkippedUpdates)) {
            await OnUpdate(simulatedTime);
            simulatedTime += UpdateInterval;
            updatesSkipped++;
        }

        CurrentInterpolation = Math.Clamp((float) ((realTime + UpdateInterval - simulatedTime) / UpdateInterval), 0, 1);
    }
}
