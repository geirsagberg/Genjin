using System.Drawing;
using System.Numerics;
using Peridot.Veldrid;
using Veldrid;
using Rectangle = System.Drawing.Rectangle;

namespace Genjin.Core;

public class ShapeBatch
{
    private readonly VeldridSpriteBatch spriteBatch;
    private TextureWrapper WhitePixelTexture { get; }

    public ShapeBatch(VeldridSpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        this.spriteBatch = spriteBatch;

        WhitePixelTexture = CreateWhitePixelTexture(graphicsDevice);
    }

    private static TextureWrapper CreateWhitePixelTexture(GraphicsDevice graphicsDevice)
    {
        var textureDescription = new TextureDescription(1, 1, 1, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.Sampled, TextureType.Texture2D);
        var texture = graphicsDevice.ResourceFactory.CreateTexture(textureDescription);
        graphicsDevice.UpdateTexture(texture, new byte[] { 255, 255, 255, 255 }, 0, 0, 0, 1, 1, 1, 0, 0);
        return new TextureWrapper(texture);
    }

    public void FillRectangle(Rectangle rectangle, Color color)
    {
        spriteBatch.Draw(WhitePixelTexture, rectangle, new Rectangle(0, 0, 1, 1), color, 0,
            Vector2.Zero,
            0);
    }

    public void DrawPolygon(Vector2 offset, IReadOnlyList<Vector2> points, Color color, float thickness = 1f,
        float layerDepth = 0f)
    {
        if (points.Count == 0) return;
        if (points.Count == 1) {
            DrawPoint(points[0], color, thickness);
        } else {
        }
    }

    public void DrawPoint(Vector2 position, Color color, float size = 1f, float layerDepth = 0f)
    {
        var scale = Vector2.One * size;
        var offset = new Vector2(0.5f) - new Vector2(size * 0.5f);
        spriteBatch.Draw(WhitePixelTexture, position + offset, Rectangle.Empty, color, 0, Vector2.Zero, scale,
            layerDepth);
    }
}