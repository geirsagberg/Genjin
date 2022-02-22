using System.Drawing;
using Genjin.Example;
using Peridot.Veldrid;

namespace Genjin.Core;

public static class SpriteBatchExtensions
{
    public static void DrawSprite(this VeldridSpriteBatch spriteBatch, TextureWrapper textureWrapper,
        Transform2 transform) =>
        spriteBatch.Draw(textureWrapper, transform.Rectangle, new Rectangle(Point.Empty, textureWrapper.Size),
            Color.White,
            transform.Rotation, transform.Origin, 0f);

    public static void DrawSprite(this VeldridSpriteBatch spritebatch, SpriteSheet spriteSheet, int column, int row,
        Transform2 transform) =>
        spritebatch.Draw(spriteSheet.Texture, transform.Rectangle, spriteSheet.GetSpriteRectangle(column, row),
            Color.White, transform.Rotation, transform.Origin, 0f);
}