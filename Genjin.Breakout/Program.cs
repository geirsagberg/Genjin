using Genjin.Core;
using Microsoft.Extensions.DependencyInjection;
using Veldrid;

await new BreakoutGame().Start();

internal class SceneManager {
    public IScene CurrentScene { get; private set; } = new DefaultScene();

    public async Task SetScene(IScene newScene) {
        CurrentScene = newScene;
        await CurrentScene.OnLoad();
    }
}

internal class BreakoutGame : Game {
    protected SceneManager SceneManager => Services.GetRequiredService<SceneManager>();

    protected override void ConfigureServices(IServiceCollection services) {
        services.AddSingleton<SceneManager>();
    }

    protected override async Task Init() {
        await SceneManager.SetScene(new MenuScene());
    }

    protected override Task UpdateBasedOnInput(InputSnapshot input) {
        return Task.CompletedTask;
    }

    protected override void Draw(TimeSpan sincePreviousFrame) {
    }
}

internal interface IScene {
    Task OnLoad();
}

internal class DefaultScene : IScene {
    public Task OnLoad() {
        return Task.CompletedTask;
    }
}

internal class MenuScene : IScene {
    public Task OnLoad() {
        return Task.CompletedTask;
    }
}
