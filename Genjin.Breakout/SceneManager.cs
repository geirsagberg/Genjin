using Genjin.Core;

namespace Genjin.Breakout;

internal class SceneManager : IDrawable {
    public IScene CurrentScene { get; private set; } = new DefaultScene();

    public void Draw() => CurrentScene.Draw();

    public async Task SetScene(IScene newScene) {
        CurrentScene = newScene;
        await CurrentScene.OnLoad();
    }
}
