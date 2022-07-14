using System.Numerics;
using Genjin.Core;
using Peridot;
using Peridot.Veldrid;

namespace Genjin.Example;

internal class MyGame : Game {
    private const double FpsSmoothing = 0.9;

    private AnimatedSprite<PlayerState> animatedSprite = null!;

    private TimeSpan animationTime;

    private int currentAnimationFrame;

    private PlayerState currentPlayerState;

    private int draws;

    private Facing facing = Facing.Right;

    private double fps;
    private TextureWrapper playerSprite = null!;
    private Simulation simulation = null!;

    private SpriteSheet spriteSheet = null!;
    private int updates;

    protected override Task Init() {
        World.AddSimulationSystems();

        playerSprite = LoadTexture("Assets/Sprites/player.png");
        spriteSheet = new SpriteSheet(playerSprite, 21, 1);
        animatedSprite = new AnimatedSprite<PlayerState>(spriteSheet, new Dictionary<PlayerState, Animation> {
            { PlayerState.Idle, new(..0) },
            { PlayerState.Running, new(1..9) }
        });

        transform = new Body(new Vector2(100, 100), spriteSheet.SpriteSize);

        simulation = StartSimulation();

        return Task.CompletedTask;
    }

    protected override void Draw(TimeSpan deltaTime) {
        var currentFps = 1.0 / deltaTime.TotalSeconds;
        fps = (fps * FpsSmoothing) + (currentFps * (1.0 - FpsSmoothing));

        animationTime += deltaTime;

        var newPlayerState = pressedKeys.Any() ? PlayerState.Running : PlayerState.Idle;

        var animation = animatedSprite.Animations[newPlayerState];

        if (newPlayerState == PlayerState.Running) {
            if (currentPlayerState != PlayerState.Running) {
                currentAnimationFrame = 0;
            }

            currentPlayerState = newPlayerState;
            var animationFrameInterval = TimeSpan.FromSeconds(1 / animation.Fps);
            while (animationTime > animationFrameInterval) {
                currentAnimationFrame++;
                animationTime -= animationFrameInterval;
            }
        }

        currentAnimationFrame %= animation.Frames.GetLength();

        var viewPosition = transform.Position +
            (velocity * GetFactor(simulation.UpdateInterval) * simulation.CurrentInterpolation);

        var spriteSheetIndex = animation.Frames.Start.Value + currentAnimationFrame;

        SpriteBatch.DrawSprite(spriteSheet, spriteSheetIndex,
            transform with {
                Position = viewPosition
            }, facing == Facing.Right ? SpriteOptions.None : SpriteOptions.FlipHorizontally);

        DrawString($"{transform.Position.X:F2}", Vector2.Zero);
        DrawString($"{transform.Position.Y:F2}", new Vector2(150, 0));
        DrawString($"Draws: {draws++}", new Vector2(0, 40));
        DrawString($"FPS: {fps:F0}", new Vector2(200, 40));
        DrawString($"Updates: {updates}", new Vector2(0, 80));
    }

    protected ValueTask UpdatePhysics(TimeSpan physicsInterval) {
        var newPosition = transform.Position + (velocity * GetFactor(physicsInterval));
        transform = transform with {
            Position = newPosition.Wrap(Window.Bounds.Size)
        };
        updates++;

        return ValueTask.CompletedTask;
    }

    private static float GetFactor(TimeSpan interval) => (float) interval.TotalSeconds * 200;
}

internal enum GameKey : byte {
    Up,
    Down,
    Left,
    Right
}
