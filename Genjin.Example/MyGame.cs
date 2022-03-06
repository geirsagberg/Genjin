using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using Genjin.Core;
using Peridot.Veldrid;
using Veldrid;
using Rectangle = System.Drawing.Rectangle;

namespace Genjin.Example;

internal interface IDrawable
{
}

internal class MyGame : Game
{
    private TextureWrapper playerSprite;
    private Transform2 transform;
    private Font arial = null!;
    private AnimatedSprite animatedSprite = null!;
    private SpriteSheet spriteSheet = null!;


    protected override void Init()
    {
        playerSprite = LoadTexture("Assets/Sprites/player.png");

        spriteSheet = new SpriteSheet(playerSprite, 21, 1);
        animatedSprite = new AnimatedSprite(spriteSheet, new Dictionary<string, Animation> {
            { "idle", new(..0) },
            { "running", new(1..9) }
        });

        arial = LoadFont("Assets/Fonts/arial.ttf");
        transform = new Transform2(new Vector2(100, 100), default, spriteSheet.SpriteSize);
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

    protected override async Task UpdateBasedOnInput(InputSnapshot input)
    {
        UpdatePressedKeys(input);

        var newVelocity =
            new Vector2((pressedKeys.Contains(GameKey.Left) ? -1f : 0) + (pressedKeys.Contains(GameKey.Right) ? 1f : 0),
                (pressedKeys.Contains(GameKey.Up) ? -1f : 0) + (pressedKeys.Contains(GameKey.Down) ? 1f : 0));

        velocity = newVelocity.NormalizeOrZero();
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

    protected override void DrawSprites(Stopwatch realTime, double interpolation, TimeSpan physicsInterval)
    {
        var timeToDrawPreviousFrame = realTime.Elapsed - previousRealTime;
        var currentFps = 1.0 / timeToDrawPreviousFrame.TotalSeconds;
        fps = fps * FpsSmoothing + currentFps * (1.0 - FpsSmoothing);

        previousRealTime = realTime.Elapsed;

        var viewPosition = transform.Position + velocity * GetFactor(physicsInterval) * (float)interpolation;

        SpriteBatch.DrawSprite(spriteSheet, 0, 0, transform with { Position = viewPosition });
        DrawString($"{transform.Position.X:F2}", Vector2.Zero);
        DrawString($"{transform.Position.Y:F2}", new Vector2(150, 0));
        DrawString($"Draws: {draws++}", new Vector2(0, 40));
        DrawString($"FPS: {fps:F0}", new Vector2(200, 40));
        DrawString($"Updates: {updates}", new Vector2(0, 80));

        FillRectangle(new Rectangle(100, 100, 200, 200), Color.Aqua);
    }

    private void DrawString(string text, Vector2 position)
    {
        TextRenderer.DrawString(arial, 32, text, position, Color.Aqua, 0f, Vector2.Zero,
            Vector2.One, 0f);
    }

    private Vector2 velocity = Vector2.Zero;

    protected override async Task UpdatePhysics(TimeSpan physicsInterval)
    {
        var newPosition = transform.Position + velocity * GetFactor(physicsInterval);
        transform = transform with {
            Position = Wrap(newPosition, Window.Bounds.Size)
        };
        updates++;
    }

    private static float GetFactor(TimeSpan interval)
        => (float)interval.TotalSeconds * 1000;

    private static Vector2 Wrap(Vector2 position, Vector2 size)
        => new(Wrap(position.X, size.X), Wrap(position.Y, size.Y));

    private static float Wrap(float position, float max)
        => position < 0 ? position + max : position >= max ? position - max : position;
}