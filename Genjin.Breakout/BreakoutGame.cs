using Genjin.Breakout.Components;
using Genjin.Core;
using Microsoft.Extensions.DependencyInjection;
using Veldrid;

namespace Genjin.Breakout;

internal class BreakoutGame : Game {
    private readonly MenuScene menuScene;

    private readonly World world = new();

    public BreakoutGame() {
        menuScene = new MenuScene(MessageHub);
    }

    protected SceneManager SceneManager => Services.GetRequiredService<SceneManager>();

    protected override void ConfigureServices(IServiceCollection services) => services.AddSingleton<SceneManager>();

    protected override async Task Init() {
        Window.Title = "Breakout";

        world.AddSystem(new RenderSystem(ShapeRenderer, world));

        MessageHub.Subscribe<StartGameMessage>(_ => SceneManager.SetScene(new GameScene(world, ShapeRenderer)));
        // await SceneManager.SetScene(menuScene);
        await SceneManager.SetScene(new GameScene(world, ShapeRenderer));
    }

    protected override Task UpdateBasedOnInput(InputSnapshot input) => Task.CompletedTask;

    protected override void Draw(TimeSpan sincePreviousFrame) {
        SceneManager.Draw();
        world.Draw();
    }
}
