using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using Genjin.Core;
using Peridot.Veldrid;

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

    private readonly double fpsSmoothing = 0.9;

    private TimeSpan previousRealTime = TimeSpan.Zero;

    protected override void DrawSprites(Stopwatch realTime)
    {
        var timeToDrawPreviousFrame = realTime.Elapsed - previousRealTime;
        var currentFps = 1.0 / timeToDrawPreviousFrame.TotalSeconds;
        fps = fps * fpsSmoothing + currentFps * (1.0 - fpsSmoothing);

        previousRealTime = realTime.Elapsed;

        SpriteBatch.DrawSprite(spriteSheet, 0, 0, transform);
        DrawString(TextRenderer, $"{transform.Position.X}", Vector2.Zero);
        TextRenderer.DrawString(arial, 32, $"{transform.Position.Y}", new Vector2(150, 0), Color.Aqua, 0f, Vector2.Zero,
            Vector2.One, 0f);
        TextRenderer.DrawString(arial, 32, $"Draws: {draws++}", new Vector2(0, 40), Color.Aqua, 0f, Vector2.Zero,
            Vector2.One, 0f);
        TextRenderer.DrawString(arial, 32, $"FPS: {fps}", new Vector2(200, 40), Color.Aqua, 0f, Vector2.Zero,
            Vector2.One, 0f);
        TextRenderer.DrawString(arial, 32, $"Updates: {updates}", new Vector2(0, 80), Color.Aqua, 0f, Vector2.Zero,
            Vector2.One, 0f);
    }

    private void DrawString(TextRenderer textRenderer, string text, Vector2 position)
    {
        textRenderer.DrawString(arial, 32, text, position, Color.Aqua, 0f, Vector2.Zero,
            Vector2.One, 0f);
    }

    protected override async Task Update(TimeSpan interval)
    {
        Console.WriteLine($"{transform.Position}");
        var change = interval.TotalMilliseconds / 5;
        var newX = transform.Position.X + change;
        transform = transform with {
            Position = transform.Position with {
                X = (float)(newX % Window.Width),
                Y = (float)((transform.Position.Y + change) % Window.Height)
            }
        };
        updates++;
    }
}