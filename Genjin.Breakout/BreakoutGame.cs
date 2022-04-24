using Genjin.Core;
using Microsoft.Extensions.DependencyInjection;
using Veldrid;

namespace Genjin.Breakout;

internal class BreakoutGame : Game {
    protected SceneManager SceneManager => Services.GetRequiredService<SceneManager>();

    protected override void ConfigureServices(IServiceCollection services) {
        services.AddSingleton<SceneManager>();
    }

    protected override async Task Init() {
        Window.Title = "Breakout";
        await SceneManager.SetScene(new MenuScene(Stop));
    }

    protected override Task UpdateBasedOnInput(InputSnapshot input) {
        return Task.CompletedTask;
    }

    protected override void Draw(TimeSpan sincePreviousFrame) {
        SceneManager.Draw();
    }
}
