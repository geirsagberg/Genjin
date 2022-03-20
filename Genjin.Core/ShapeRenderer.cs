using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Peridot.Veldrid;
using Veldrid;
using Rectangle = System.Drawing.Rectangle;

namespace Genjin.Core;

public class ShapeRenderer {
    private static readonly Rectangle SinglePixelRectangle = new(0, 0, 1, 1);
    private readonly VeldridSpriteBatch spriteBatch;

    public ShapeRenderer(GraphicsDevice graphicsDevice, VeldridSpriteBatch spriteBatch) {
        this.spriteBatch = spriteBatch;

        WhitePixelTexture = CreateWhitePixelTexture(graphicsDevice);
    }

    private TextureWrapper WhitePixelTexture { get; }

    private static TextureWrapper CreateWhitePixelTexture(GraphicsDevice graphicsDevice) {
        var textureDescription = new TextureDescription(1, 1, 1, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.Sampled, TextureType.Texture2D);
        var texture = graphicsDevice.ResourceFactory.CreateTexture(textureDescription);
        graphicsDevice.UpdateTexture(texture, new byte[] { 255, 255, 255, 255 }, 0, 0, 0, 1, 1, 1, 0, 0);
        return new TextureWrapper(texture);
    }

    public void FillRectangle(Rectangle rectangle, Color color) =>
        spriteBatch.Draw(WhitePixelTexture, rectangle, SinglePixelRectangle, color, 0, Vector2.Zero, 0);

    public void DrawPolygon(Vector2 offset, IReadOnlyList<Vector2> points, Color color, float thickness = 1f,
        float layerDepth = 0f) {
        switch (points.Count) {
            case 0:
                break;
            case 1:
                DrawPoint(points[0], color, thickness);
                break;
            default: {
                for (var i = 0; i < points.Count - 1; i++) {
                    DrawPolygonEdge(points[i] + offset, points[i + 1] + offset, color, thickness, layerDepth);
                }

                DrawPolygonEdge(points[^1] + offset, points[0] + offset, color, thickness, layerDepth);
                break;
            }
        }
    }

    public void DrawPoint(Vector2 position, Color color, float size = 1f, float layerDepth = 0f) {
        var scale = Vector2.One * size;
        var offset = new Vector2(0.5f) - new Vector2(size * 0.5f);
        spriteBatch.Draw(WhitePixelTexture, position + offset, SinglePixelRectangle, color, 0, Vector2.Zero, scale,
            layerDepth);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Angle(Vector2 point1, Vector2 point2) =>
        MathF.Atan2(point2.Y - point1.Y, point2.X - point1.X);

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

    private void DrawPolygonEdge(Vector2 point1, Vector2 point2, Color color, float thickness, float layerDepth) {
        var vector = Vector2.Distance(point1, point2);
        var rotation = Angle(point1, point2);
        var scaled = new Vector2(vector, thickness);
        spriteBatch.Draw(WhitePixelTexture, point1, SinglePixelRectangle, color, rotation, Vector2.Zero, scaled,
            layerDepth);
    }

    public void DrawLine(Vector2 start, float length, float angle, Color color, float thickness = 1,
        float layerDepth = 0) {
        var origin = new Vector2(0, 0.5f);
        var scale = new Vector2(length, thickness);
        spriteBatch.Draw(WhitePixelTexture, start, SinglePixelRectangle, color, angle, origin, scale, layerDepth);
    }

    public void DrawLine(Vector2 start, Vector2 end, Color color, float thickness = 1, float layerDepth = 0) {
        var length = Vector2.Distance(start, end);
        var angle = Angle(start, end);
        DrawLine(start, length, angle, color, thickness, layerDepth);
    }

    public void DrawCircle(Vector2 center, float radius, int sides, Color color, float thickness = 1,
        float layerDepth = 0) =>
        DrawPolygon(center, CreateCircle(radius, sides), color, thickness, layerDepth);

    public void DrawEllipse(Vector2 center, Vector2 radius, int sides, Color color, float thickness = 1,
        float layerDepth = 0) =>
        DrawPolygon(center, CreateEllipse(radius.X, radius.Y, sides), color, thickness, layerDepth);

    private static Vector2[] CreateEllipse(float radiusX, float radiusY, int sides) {
        var ellipsePoints = new Vector2[sides];
        var deltaAngle = MathF.Tau / sides;
        var currentAngle = 0f;
        for (var i = 0; i < sides; i++) {
            var x = radiusX * MathF.Cos(currentAngle);
            var y = radiusY * MathF.Sin(currentAngle);
            ellipsePoints[i] = new Vector2(x, y);
            currentAngle += deltaAngle;
        }

        return ellipsePoints;
    }

    private static Vector2[] CreateCircle(float radius, int sides) {
        var circlePoints = new Vector2[sides];
        var deltaAngle = MathF.Tau / sides;
        var currentAngle = 0f;
        for (var i = 0; i < sides; i++) {
            circlePoints[i] = new Vector2(radius * MathF.Cos(currentAngle), radius * MathF.Sin(currentAngle));
            currentAngle += deltaAngle;
        }

        return circlePoints;
    }
}
