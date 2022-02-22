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

    protected override void DrawSprites(VeldridSpriteBatch spriteBatch, TextRenderer textRenderer)
    {
        spriteBatch.DrawSprite(spriteSheet, 0, 0, transform);
        textRenderer.DrawString(arial, 32, $"{transform.Position}", Vector2.Zero, Color.Aqua, 0f, Vector2.Zero,
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
    }
}