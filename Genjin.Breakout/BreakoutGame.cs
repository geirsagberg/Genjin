using Genjin.Breakout.Components;
using Genjin.Core;
using Genjin.Core.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Genjin.Breakout;

internal class PhysicsSystem : ISimulationSystem {
    public PhysicsSystem() {
    }

    public ValueTask Update(TimeSpan simulatedTime) => throw new NotImplementedException();
}

internal interface IEntitySystem {
    Aspect Aspect { get; }
}

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

        world
            .AddSimulationSystems()
            .AddSystem(new RenderSystem(ShapeRenderer, world));


        MessageHub.Subscribe<StartGameMessage>(_ => SceneManager.SetScene(CreateGameScene()));
        // await SceneManager.SetScene(menuScene);
        await SceneManager.SetScene(CreateGameScene());
    }

    private GameScene CreateGameScene() => new(world, ShapeRenderer, StartSimulation());

    protected override void Draw(TimeSpan sincePreviousFrame) {
        SceneManager.Draw();
        world.Draw();
    }
}
