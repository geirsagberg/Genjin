using System.Drawing;
using System.Numerics;
using Genjin.Core.Primitives;
using Genjin.Example;
using Peridot;
using Peridot.Veldrid;

namespace Genjin.Core;

public static class SpriteBatchExtensions {
    public static void DrawSprite(this VeldridSpriteBatch spriteBatch, TextureWrapper textureWrapper,
        System.Drawing.Rectangle sourceRectangle, IShape shape, SpriteOptions spriteEffects = SpriteOptions.None,
        float layerDepth = 0) =>
        spriteBatch.Draw(textureWrapper, shape.Position.Round(), sourceRectangle, Color.White,
            0, Vector2.Zero, Vector2.One, spriteEffects, layerDepth);

    public static void DrawSprite(this VeldridSpriteBatch spriteBatch, TextureWrapper textureWrapper,
        IShape shape, SpriteOptions spriteEffects = SpriteOptions.None, float layerDepth = 0) =>
        spriteBatch.DrawSprite(textureWrapper, new System.Drawing.Rectangle(Point.Empty, textureWrapper.Size), shape,
            spriteEffects, layerDepth);

    public static void DrawSprite(this VeldridSpriteBatch spritebatch, SpriteSheet spriteSheet, int column, int row,
        IShape shape, SpriteOptions spriteEffects = SpriteOptions.None, float layerDepth = 0) =>
        spritebatch.DrawSprite(spriteSheet.Texture, spriteSheet.GetSpriteRectangle(column, row), shape,
            spriteEffects, layerDepth);

    public static void DrawSprite(this VeldridSpriteBatch spritebatch, SpriteSheet spriteSheet, int spriteSheetIndex,
        IShape shape, SpriteOptions spriteEffects = SpriteOptions.None) {
        var (row, column) = Math.DivRem(spriteSheetIndex, spriteSheet.Columns);
        spritebatch.DrawSprite(spriteSheet, column, row, shape, spriteEffects);
    }
}
