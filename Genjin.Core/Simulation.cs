namespace Genjin.Core;

public class Simulation {
    private readonly ushort maxSkippedUpdates;
    private readonly Func<TimeSpan, Task> onUpdate;

    private TimeSpan simulatedTime;

    public Simulation(Func<TimeSpan, Task> onUpdate, TimeSpan startTime = default, ushort updatesPerSecond = 60,
        ushort maxSkippedUpdates = 5) {
        if (updatesPerSecond == 0) throw new ArgumentOutOfRangeException(nameof(updatesPerSecond), "Must be non-zero");
        this.onUpdate = onUpdate;
        this.maxSkippedUpdates = maxSkippedUpdates;
        simulatedTime = startTime;
        UpdateInterval = TimeSpan.FromSeconds(1.0 / updatesPerSecond);
    }

    public float CurrentInterpolation { get; private set; }

    public TimeSpan UpdateInterval { get; }

    public async Task Update(TimeSpan realTime) {
        var updatesSkipped = 0;
        while (simulatedTime < realTime && (maxSkippedUpdates == 0 || updatesSkipped < maxSkippedUpdates)) {
            await onUpdate(UpdateInterval);
            simulatedTime += UpdateInterval;
            updatesSkipped++;
        }

        CurrentInterpolation = Math.Clamp((float)((realTime + UpdateInterval - simulatedTime) / UpdateInterval), 0, 1);
    }
}
