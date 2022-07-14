using System.Drawing;
using Genjin.Core.Primitives;
using Genjin.Example;
using Peridot;
using Peridot.Veldrid;

namespace Genjin.Core;

public static class SpriteBatchExtensions {
    public static void DrawSprite(this VeldridSpriteBatch spriteBatch, TextureWrapper textureWrapper,
        Rectangle sourceRectangle, Body body, SpriteOptions spriteEffects = SpriteOptions.None,
        float layerDepth = 0) =>
        spriteBatch.Draw(textureWrapper, body.Position.Round(), sourceRectangle, Color.White,
            body.Rotation, body.Origin, body.Scale, spriteEffects, layerDepth);

    public static void DrawSprite(this VeldridSpriteBatch spriteBatch, TextureWrapper textureWrapper,
        Body body, SpriteOptions spriteEffects = SpriteOptions.None, float layerDepth = 0) =>
        spriteBatch.DrawSprite(textureWrapper, new Rectangle(Point.Empty, textureWrapper.Size), body,
            spriteEffects, layerDepth);

    public static void DrawSprite(this VeldridSpriteBatch spritebatch, SpriteSheet spriteSheet, int column, int row,
        Body body, SpriteOptions spriteEffects = SpriteOptions.None, float layerDepth = 0) =>
        spritebatch.DrawSprite(spriteSheet.Texture, spriteSheet.GetSpriteRectangle(column, row), body,
            spriteEffects, layerDepth);

    public static void DrawSprite(this VeldridSpriteBatch spritebatch, SpriteSheet spriteSheet, int spriteSheetIndex,
        Body body, SpriteOptions spriteEffects = SpriteOptions.None) {
        var (row, column) = Math.DivRem(spriteSheetIndex, spriteSheet.Columns);
        spritebatch.DrawSprite(spriteSheet, column, row, body, spriteEffects);
    }
}
