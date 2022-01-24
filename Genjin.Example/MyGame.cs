using System.Diagnostics;
using Genjin.Core;
using Peridot.Veldrid;
using StbImageSharp;
using Veldrid;

namespace Genjin.Example;

internal interface IDrawable
{
}

internal class MyGame : Game
{
    private TextureWrapper playerSprite;

    public async Task Init()
    {
        playerSprite = await LoadTexture("Resources/Sprites/player.png");
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }

    public void Draw(GameTime gameTime, VeldridSpriteBatch spriteBatch)
    {
        var transform = new Transform2(default, default, playerSprite.Size);
        spriteBatch.DrawSprite(playerSprite, transform);
    }

    private async Task<TextureWrapper> LoadTexture(string path)
    {
        var bytes = await File.ReadAllBytesAsync(path);
        var image = ImageResult.FromMemory(bytes);
        Debug.Assert(image != null);
        var textureDescription = new TextureDescription(
            (uint)image.Width, (uint)image.Height,
            1, 1, 1,
            PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.Sampled,
            TextureType.Texture2D
        );
        var texture = ResourceFactory.CreateTexture(textureDescription);
        GraphicsDevice.UpdateTexture(texture, image.Data, 0, 0, 0, textureDescription.Width, textureDescription.Height,
            textureDescription.Depth, 0, 0);

        return new TextureWrapper(texture);
    }
}