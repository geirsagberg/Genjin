using System.Drawing;
using Genjin.Example;
using Peridot;
using Peridot.Veldrid;

namespace Genjin.Core;

public static class SpriteBatchExtensions {
    public static void DrawSprite(this VeldridSpriteBatch spriteBatch, TextureWrapper textureWrapper,
        Rectangle sourceRectangle, Transform transform, SpriteOptions spriteEffects = SpriteOptions.None,
        float layerDepth = 0) =>
        spriteBatch.Draw(textureWrapper, transform.Position.Round(), sourceRectangle, Color.White,
            transform.Rotation, transform.Origin, transform.Scale, spriteEffects, layerDepth);

    public static void DrawSprite(this VeldridSpriteBatch spriteBatch, TextureWrapper textureWrapper,
        Transform transform, SpriteOptions spriteEffects = SpriteOptions.None, float layerDepth = 0) =>
        spriteBatch.DrawSprite(textureWrapper, new Rectangle(Point.Empty, textureWrapper.Size), transform,
            spriteEffects, layerDepth);

    public static void DrawSprite(this VeldridSpriteBatch spritebatch, SpriteSheet spriteSheet, int column, int row,
        Transform transform, SpriteOptions spriteEffects = SpriteOptions.None, float layerDepth = 0) =>
        spritebatch.DrawSprite(spriteSheet.Texture, spriteSheet.GetSpriteRectangle(column, row), transform,
            spriteEffects, layerDepth);

    public static void DrawSprite(this VeldridSpriteBatch spritebatch, SpriteSheet spriteSheet, int spriteSheetIndex,
        Transform transform, SpriteOptions spriteEffects = SpriteOptions.None) {
        var (row, column) = Math.DivRem(spriteSheetIndex, spriteSheet.Columns);
        spritebatch.DrawSprite(spriteSheet, column, row, transform, spriteEffects);
    }
}
