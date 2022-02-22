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


    protected override void Init()
    {
        playerSprite = LoadTexture("Assets/Sprites/player.png");
        arial = LoadFont("Assets/Fonts/arial.ttf");
        transform = new Transform2(new Vector2(100, 100), default, playerSprite.Size);
    }

    protected override void DrawSprites(VeldridSpriteBatch spriteBatch, TextRenderer textRenderer)
    {
        spriteBatch.DrawSprite(playerSprite, transform);
        textRenderer.DrawString(arial, 32, $"{transform.Position}", Vector2.Zero, Color.Aqua, 0f, Vector2.Zero,
            Vector2.One, 0f);
    }

    protected override async Task Update(TimeSpan interval)
    {
        Console.WriteLine($"{transform.Position}");
        var newX = transform.Position.X + interval.TotalMilliseconds;
        transform = transform with {
            Position = transform.Position with {
                X = (float)(newX % Window.Width),
                Y = (float)((transform.Position.Y + interval.TotalMilliseconds) % Window.Height)
            }
        };
    }
}