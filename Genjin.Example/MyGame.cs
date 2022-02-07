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


    protected override void Init()
    {
        playerSprite = LoadTexture("Resources/Sprites/player.png");

        transform = new Transform2(default, default, playerSprite.Size);
    }

    protected override void DrawSprites(GameTime gameTime, VeldridSpriteBatch spriteBatch)
    {
        spriteBatch.DrawSprite(playerSprite, transform);
    }

    protected override void Update(GameTime gameTime)
    {
        transform = transform with {
            Position = transform.Position with {
                X = (float)((transform.Position.X + gameTime.ElapsedMilliseconds) / 10 % Window.Width)
            }
        };
    }
}