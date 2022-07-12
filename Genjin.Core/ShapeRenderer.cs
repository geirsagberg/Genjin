using System.Drawing;
using System.Numerics;
using Genjin.Core.Primitives;
using Peridot.Veldrid;
using Veldrid;
using Rectangle = System.Drawing.Rectangle;
using RectangleF = Genjin.Core.Primitives.RectangleF;

namespace Genjin.Core;

public class ShapeRenderer : IShapeRenderer {
    private static readonly Rectangle SinglePixelRectangle = new(0, 0, 1, 1);
    private readonly VeldridSpriteBatch spriteBatch;

    public ShapeRenderer(GraphicsDevice graphicsDevice, VeldridSpriteBatch spriteBatch) {
        this.spriteBatch = spriteBatch;

        WhitePixelTexture = CreateWhitePixelTexture(graphicsDevice);
    }

    private TextureWrapper WhitePixelTexture { get; }

    public void FillRectangle(RectangleF rectangle, Color color, float layerDepth = 0) {
        var rect = new Rectangle((int) rectangle.X, (int) rectangle.Y, (int) rectangle.Width, (int) rectangle.Height);
        spriteBatch.Draw(WhitePixelTexture, rect, SinglePixelRectangle, color, 0, Vector2.Zero, layerDepth);
    }

    public void DrawPoint(Vector2 position, Color color, float size = 1f, float layerDepth = 0f) {
        var scale = Vector2.One * size;
        var offset = new Vector2(0.5f) - new Vector2(size * 0.5f);
        spriteBatch.Draw(WhitePixelTexture, position + offset, SinglePixelRectangle, color, 0, Vector2.Zero, scale,
            layerDepth);
    }

    public void DrawRectangle(RectangleF rectangle, Color color, float thickness = 1, float layerDepth = 0) {
        var topLeft = new Vector2(rectangle.X, rectangle.Y);
        var topRight = new Vector2(rectangle.Right - thickness, rectangle.Y);
        var bottomLeft = new Vector2(rectangle.X, rectangle.Bottom - thickness);
        var horizontalScale = new Vector2(rectangle.Width, thickness);
        var verticalScale = new Vector2(thickness, rectangle.Height);
        spriteBatch.Draw(WhitePixelTexture, topLeft, SinglePixelRectangle, color, 0, Vector2.Zero, horizontalScale,
            layerDepth);
        spriteBatch.Draw(WhitePixelTexture, topLeft, SinglePixelRectangle, color, 0, Vector2.Zero, verticalScale,
            layerDepth);
        spriteBatch.Draw(WhitePixelTexture, topRight, SinglePixelRectangle, color, 0, Vector2.Zero, verticalScale,
            layerDepth);
        spriteBatch.Draw(WhitePixelTexture, bottomLeft, SinglePixelRectangle, color, 0, Vector2.Zero, horizontalScale,
            layerDepth);
    }

    public void DrawLine(Vector2 start, float length, float angle, Color color, float thickness = 1,
        float layerDepth = 0) {
        var origin = new Vector2(0, 0.5f);
        var scale = new Vector2(length, thickness);
        spriteBatch.Draw(WhitePixelTexture, start, SinglePixelRectangle, color, angle, origin, scale, layerDepth);
    }

    public void DrawPolygonEdge(Vector2 point1, Vector2 point2, Color color, float thickness, float layerDepth) {
        var vector = Vector2.Distance(point1, point2);
        var rotation = point1.AngleBetween(point2);
        var scaled = new Vector2(vector, thickness);
        spriteBatch.Draw(WhitePixelTexture, point1, SinglePixelRectangle, color, rotation, Vector2.Zero, scaled,
            layerDepth);
    }

    private static TextureWrapper CreateWhitePixelTexture(GraphicsDevice graphicsDevice) {
        var textureDescription = new TextureDescription(1, 1, 1, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.Sampled, TextureType.Texture2D);
        var texture = graphicsDevice.ResourceFactory.CreateTexture(textureDescription);
        graphicsDevice.UpdateTexture(texture, new byte[] { 255, 255, 255, 255 }, 0, 0, 0, 1, 1, 1, 0, 0);
        return new TextureWrapper(texture);
    }
}
