using System.Drawing;
using Peridot.Veldrid;

namespace Genjin.Example;

public static class SpriteBatchExtensions
{
    public static void DrawSprite(this VeldridSpriteBatch spriteBatch, TextureWrapper textureWrapper,
        Transform2 transform)
    {
        spriteBatch.Draw(textureWrapper, transform.Rectangle, new Rectangle(Point.Empty, textureWrapper.Size),
            Color.White,
            transform.Rotation, transform.Origin, 0f);
    }
}