﻿using System.Drawing;
using Genjin.Example;
using Peridot.Veldrid;

namespace Genjin.Core;

public static class SpriteBatchExtensions
{
    public static void DrawSprite(this VeldridSpriteBatch spriteBatch, TextureWrapper textureWrapper,
        Rectangle sourceRectangle, Transform2 transform, SpriteEffects spriteEffects = SpriteEffects.None,
        float layerDepth = 0) =>
        spriteBatch.Draw(textureWrapper, transform.Rectangle, sourceRectangle, Color.White,
            transform.Rotation, transform.Origin, layerDepth);

    // TODO FIX
    // public static void DrawSprite(this VeldridSpriteBatch spriteBatch, TextureWrapper textureWrapper,
    //     Rectangle sourceRectangle, Transform2 transform, SpriteEffects spriteEffects = SpriteEffects.None,
    //     float layerDepth = 0) =>
    //     spriteBatch.Draw(textureWrapper, transform.Position, sourceRectangle, Color.White,
    //         transform.Rotation, transform.Origin,
    //         transform.Scale * new Vector2(spriteEffects.HasFlag(SpriteEffects.FlipHorizontally) ? -1f : 1f,
    //             spriteEffects.HasFlag(SpriteEffects.FlipVertically) ? -1f : 1f), layerDepth);

    public static void DrawSprite(this VeldridSpriteBatch spriteBatch, TextureWrapper textureWrapper,
        Transform2 transform, SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0) =>
        spriteBatch.DrawSprite(textureWrapper, new Rectangle(Point.Empty, textureWrapper.Size), transform,
            spriteEffects, layerDepth);

    public static void DrawSprite(this VeldridSpriteBatch spritebatch, SpriteSheet spriteSheet, int column, int row,
        Transform2 transform, SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0) =>
        spritebatch.DrawSprite(spriteSheet.Texture, spriteSheet.GetSpriteRectangle(column, row), transform,
            spriteEffects, layerDepth);

    public static void DrawSprite(this VeldridSpriteBatch spritebatch, SpriteSheet spriteSheet, int spriteSheetIndex,
        Transform2 transform, SpriteEffects spriteEffects = SpriteEffects.None)
    {
        var (row, column) = Math.DivRem(spriteSheetIndex, spriteSheet.Columns);
        spritebatch.DrawSprite(spriteSheet, column, row, transform, spriteEffects);
    }
}