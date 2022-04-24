namespace Genjin.Breakout;

public interface IScene : IDrawable {
    Task OnLoad() => Task.CompletedTask;
}
