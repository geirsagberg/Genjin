namespace Genjin.Core;

public record Disposable(Action? Action = null) : IDisposable {
    public void Dispose() {
        GC.SuppressFinalize(this);
        Action?.Invoke();
    }
}
