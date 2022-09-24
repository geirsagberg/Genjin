using System.Drawing;
using System.Numerics;
using Genjin.Core.Primitives;
using Genjin.Example;
using Peridot;
using Veldrid;

namespace Genjin.Core;

public static class SpriteBatchExtensions {
    public static void DrawSprite(this ISpriteBatch<Texture> spriteBatch, Texture texture,
        System.Drawing.Rectangle sourceRectangle, IShape shape, SpriteOptions spriteEffects = SpriteOptions.None,
        float layerDepth = 0) =>
        spriteBatch.Draw(texture, shape.Position.Round(), sourceRectangle, Color.White,
            0, Vector2.Zero, Vector2.One, spriteEffects, layerDepth);

    public static void DrawSprite(this ISpriteBatch<Texture> spriteBatch, Texture texture,
        IShape shape, SpriteOptions spriteEffects = SpriteOptions.None, float layerDepth = 0) =>
        spriteBatch.DrawSprite(texture, new System.Drawing.Rectangle(0, 0, (int) texture.Width, (int) texture.Height),
            shape,
            spriteEffects, layerDepth);

    public static void DrawSprite(this ISpriteBatch<Texture> spritebatch, SpriteSheet spriteSheet, int column, int row,
        IShape shape, SpriteOptions spriteEffects = SpriteOptions.None, float layerDepth = 0) =>
        spritebatch.DrawSprite(spriteSheet.Texture, spriteSheet.GetSpriteRectangle(column, row), shape,
            spriteEffects, layerDepth);

    public static void DrawSprite(this ISpriteBatch<Texture> spritebatch, SpriteSheet spriteSheet, int spriteSheetIndex,
        IShape shape, SpriteOptions spriteEffects = SpriteOptions.None) {
        var (row, column) = Math.DivRem(spriteSheetIndex, spriteSheet.Columns);
        spritebatch.DrawSprite(spriteSheet, column, row, shape, spriteEffects);
    }
}
