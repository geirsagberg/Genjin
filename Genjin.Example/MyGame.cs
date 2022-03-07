using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using Genjin.Core;
using Peridot.Veldrid;
using Veldrid;
using Rectangle = System.Drawing.Rectangle;

namespace Genjin.Example;

internal enum PlayerState
{
    Idle,
    Running
}

internal enum Facing
{
    Left,
    Right
}

internal class MyGame : Game
{
    private TextureWrapper playerSprite;
    private Transform2 transform;
    private Font arial = null!;
    private AnimatedSprite<PlayerState> animatedSprite = null!;
    private SpriteSheet spriteSheet = null!;


    private TimeSpan animationTime;

    private int currentAnimationFrame;

    private PlayerState currentPlayerState;

    protected override Task Init()
    {
        playerSprite = LoadTexture("Assets/Sprites/player.png");

        spriteSheet = new SpriteSheet(playerSprite, 21, 1);
        animatedSprite = new AnimatedSprite<PlayerState>(spriteSheet, new Dictionary<PlayerState, Animation> {
            { PlayerState.Idle, new(..0) },
            { PlayerState.Running, new(1..9) }
        });

        arial = LoadFont("Assets/Fonts/arial.ttf");
        transform = new Transform2(new Vector2(100, 100), default, spriteSheet.SpriteSize);

        return Task.CompletedTask;
    }

    private int draws;
    private int updates;

    private double fps;

    private const double FpsSmoothing = 0.9;

    private TimeSpan previousRealTime = TimeSpan.Zero;

    private enum GameKey : byte
    {
        Up,
        Down,
        Left,
        Right
    }

    private readonly HashSet<GameKey> pressedKeys = new();

    private readonly Dictionary<Key, GameKey> keyMap = new() {
        { Key.A, GameKey.Left },
        { Key.W, GameKey.Up },
        { Key.S, GameKey.Down },
        { Key.D, GameKey.Right },
    };

    private Facing facing = Facing.Right;

    protected override Task UpdateBasedOnInput(InputSnapshot input)
    {
        UpdatePressedKeys(input);

        var newVelocity =
            new Vector2((pressedKeys.Contains(GameKey.Left) ? -1f : 0) + (pressedKeys.Contains(GameKey.Right) ? 1f : 0),
                (pressedKeys.Contains(GameKey.Up) ? -1f : 0) + (pressedKeys.Contains(GameKey.Down) ? 1f : 0));

        velocity = newVelocity.NormalizeOrZero();

        facing = velocity.X switch {
            > 0 => Facing.Right,
            < 0 => Facing.Left,
            _ => facing
        };

        mousePosition = input.MousePosition;

        return Task.CompletedTask;
    }

    private void UpdatePressedKeys(InputSnapshot input)
    {
        foreach (var keyEvent in input.KeyEvents) {
            if (keyMap.TryGetValue(keyEvent.Key, out var key)) {
                if (keyEvent.Down)
                    pressedKeys.Add(key);
                else
                    pressedKeys.Remove(key);
            }
        }
    }

    protected override void DrawSprites(Stopwatch realTime, float interpolation, TimeSpan physicsInterval)
    {
        var timeToDrawPreviousFrame = realTime.Elapsed - previousRealTime;
        var currentFps = 1.0 / timeToDrawPreviousFrame.TotalSeconds;
        fps = fps * FpsSmoothing + currentFps * (1.0 - FpsSmoothing);

        previousRealTime = realTime.Elapsed;

        animationTime += timeToDrawPreviousFrame;

        var newPlayerState = pressedKeys.Any() ? PlayerState.Running : PlayerState.Idle;

        var animation = animatedSprite.Animations[newPlayerState];

        if (newPlayerState == PlayerState.Running) {
            if (currentPlayerState != PlayerState.Running) {
                currentAnimationFrame = 0;
                Console.WriteLine("Reset animation");
            }

            currentPlayerState = newPlayerState;
            var animationFrameInterval = TimeSpan.FromSeconds(1 / animation.Fps);
            while (animationTime > animationFrameInterval) {
                currentAnimationFrame++;
                animationTime -= animationFrameInterval;
            }
        }

        currentAnimationFrame %= animation.Frames.GetLength();

        Console.WriteLine($"{currentAnimationFrame} {newPlayerState}");

        var viewPosition = transform.Position + velocity * GetFactor(physicsInterval) * interpolation;

        var spriteSheetIndex = animation.Frames.Start.Value + currentAnimationFrame;

        SpriteBatch.DrawSprite(spriteSheet, spriteSheetIndex,
            transform with {
                Position = viewPosition
            }, facing == Facing.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

        DrawString($"{transform.Position.X:F2}", Vector2.Zero);
        DrawString($"{transform.Position.Y:F2}", new Vector2(150, 0));
        DrawString($"Draws: {draws++}", new Vector2(0, 40));
        DrawString($"FPS: {fps:F0}", new Vector2(200, 40));
        DrawString($"Updates: {updates}", new Vector2(0, 80));

        ShapeRenderer.FillRectangle(new Rectangle((int)mousePosition.X, (int)mousePosition.Y, 20, 20), Color.Aqua);

        ShapeRenderer.DrawPoint(new Vector2(400), Color.Aqua, 1f);
        ShapeRenderer.DrawPoint(new Vector2(450, 400), Color.Aqua, 50f);

        ShapeRenderer.DrawRectangle(new RectangleF(300, 400, 50, 90), Color.Chartreuse, 3f);

        ShapeRenderer.DrawPolygon(new Vector2(600, 200),
            new Vector2[] { new(0, 0), new(30, 100), new(-60, 120) }, Color.Khaki, 10f);

        ShapeRenderer.DrawCircle(new Vector2(300, 350), 100, 32, Color.Coral, 50f);
    }

    private void DrawString(string text, Vector2 position)
    {
        TextRenderer.DrawString(arial, 32, text, position, Color.Aqua, 0f, Vector2.Zero,
            Vector2.One, 0f);
    }

    private Vector2 velocity = Vector2.Zero;
    private Vector2 mousePosition;

    protected override Task UpdatePhysics(TimeSpan physicsInterval)
    {
        var newPosition = transform.Position + velocity * GetFactor(physicsInterval);
        transform = transform with {
            Position = Wrap(newPosition, Window.Bounds.Size)
        };
        updates++;

        return Task.CompletedTask;
    }

    private static float GetFactor(TimeSpan interval)
        => (float)interval.TotalSeconds * 1000;

    private static Vector2 Wrap(Vector2 position, Vector2 size)
        => new(Wrap(position.X, size.X), Wrap(position.Y, size.Y));

    private static float Wrap(float position, float max)
        => position < 0 ? position + max : position >= max ? position - max : position;
}