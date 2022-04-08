using Genjin.Core;
using ImGuiNET;
using Microsoft.Extensions.DependencyInjection;
using Veldrid;

await new BreakoutGame().Start();

internal class SceneManager : IDrawable {
    public IScene CurrentScene { get; private set; } = new DefaultScene();

    public void Draw() {
        CurrentScene.Draw();
    }

    public async Task SetScene(IScene newScene) {
        CurrentScene = newScene;
        await CurrentScene.OnLoad();
    }
}

internal interface IDrawable {
    void Draw() { }
}

internal class BreakoutGame : Game {
    protected SceneManager SceneManager => Services.GetRequiredService<SceneManager>();

    protected override void ConfigureServices(IServiceCollection services) {
        services.AddSingleton<SceneManager>();
    }

    protected override async Task Init() {
        await SceneManager.SetScene(new MenuScene(Stop));
    }

    protected override Task UpdateBasedOnInput(InputSnapshot input) {
        return Task.CompletedTask;
    }

    protected override void Draw(TimeSpan sincePreviousFrame) {
        SceneManager.Draw();
    }
}

internal interface IScene : IDrawable {
    Task OnLoad() => Task.CompletedTask;
}

internal class DefaultScene : IScene {
}

internal class MenuScene : IScene {
    private readonly Action stop;

    public MenuScene(Action stop) {
        this.stop = stop;
    }

    public void Draw() {
        using (Gui.MainMenuBar()) {
            using (Gui.Menu("File")) {
                if (ImGui.MenuItem("Exit")) {
                    stop();
                }
            }
        }

        using (Gui.Window("",
                   ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar |
                   ImGuiWindowFlags.NoMove)) {
            if (ImGui.Button("Hello")) {
                Console.WriteLine("Hello");
            }
        }
    }
}
