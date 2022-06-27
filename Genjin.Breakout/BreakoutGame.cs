using Genjin.Breakout.Systems;
using Genjin.Core;
using Genjin.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Veldrid;

namespace Genjin.Breakout;

internal interface IEntitySystem {
    Aspect Aspect { get; }
}

internal class BreakoutGame : Game {
    private readonly MenuScene menuScene;

    public BreakoutGame() {
        menuScene = new MenuScene(MessageHub);
    }

    protected SceneManager SceneManager => Get<SceneManager>();

    protected override void ConfigureServices(IServiceCollection services) {
        services.AddSingleton<IKeyMap<GameKeys>, BreakoutKeyMap>();
        services.AddSingleton<SharedState>();
        services.AddSingleton<Provide<InputState<GameKeys>>>(provider => {
            var getInputs = provider.GetRequiredService<Provide<InputSnapshot>>();
            var inputState = provider.GetRequiredService<InputState<GameKeys>>();
            return () => {
                var inputs = getInputs();
                inputState.Update(inputs);
                return inputState;
            };
        });
        services.AddTransient<InputState<GameKeys>>();
        services.AddSingleton<SceneManager>();
    }

    protected override async Task Init() {
        Window.Title = "Breakout";

        World
            .AddSimulationSystems(Get<GameSystem>(), Get<PaddleSystem>(), Get<MovableSystem>())
            .AddSystem(new RenderSystem(ShapeRenderer, World));

        MessageHub.Subscribe<StartGameMessage>(_ => SceneManager.SetScene(Get<GameScene>()));
        // await SceneManager.SetScene(menuScene);
        await SceneManager.SetScene(Get<GameScene>());
    }

    protected override void Draw(TimeSpan deltaTime) {
        SceneManager.Draw();
        World.Draw();
    }
}
