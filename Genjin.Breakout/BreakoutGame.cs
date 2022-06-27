using Genjin.Breakout.Components;
using Genjin.Core;
using Genjin.Core.Entities;
using Genjin.Example;
using Microsoft.Extensions.DependencyInjection;
using Veldrid;

namespace Genjin.Breakout;

internal interface IEntitySystem {
    Aspect Aspect { get; }
}

internal class BreakoutGame : Game {
    private readonly MenuScene menuScene;

    private readonly World world;

    public BreakoutGame() {
        menuScene = new MenuScene(MessageHub);
        world = new World(AddUpdatable);
    }

    protected SceneManager SceneManager => Get<SceneManager>();

    protected override void ConfigureServices(IServiceCollection services) {
        services.AddSingleton<IKeyMap<GameKeys>, BreakoutKeyMap>();
        services.AddSingleton(world);
        services.AddSingleton<IEntityManager>(provider => provider.GetRequiredService<World>());
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

        world
            .AddSimulationSystems(Get<PaddleSystem>(), Get<MovableSystem>())
            .AddSystem(new RenderSystem(ShapeRenderer, world));


        MessageHub.Subscribe<StartGameMessage>(_ => SceneManager.SetScene(CreateGameScene()));
        // await SceneManager.SetScene(menuScene);
        await SceneManager.SetScene(CreateGameScene());
    }

    private GameScene CreateGameScene() => new(world, ShapeRenderer);

    protected override void Draw(TimeSpan deltaTime) {
        SceneManager.Draw();
        world.Draw();
    }
}

internal class MovableSystem : ISimulationSystem {
    private readonly IEntityManager entityManager;

    public MovableSystem(IEntityManager entityManager) {
        this.entityManager = entityManager;
    }

    public void Update(TimeSpan deltaTime) {
        var entities = entityManager.GetEntitiesMatchingAll(typeof(Movable), typeof(Transform));
        foreach (var entity in entities) {
            var movable = entity.Get<Movable>();
            var transform = entity.Get<Transform>();
            transform.Position += movable.Velocity * (float) deltaTime.TotalSeconds;
        }
    }
}
