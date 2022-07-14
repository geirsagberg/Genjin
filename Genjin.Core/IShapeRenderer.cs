using System.Drawing;
using System.Numerics;
using Genjin.Core.Primitives;
using RectangleF = Genjin.Core.Primitives.RectangleF;

namespace Genjin.Core;

public interface IShapeRenderer {

    void DrawPoint(Vector2 position, Color color, float size = 1f, float layerDepth = 0f);
    void DrawRectangle(RectangleF rectangle, Color color, float thickness = 1, float layerDepth = 0);
    void DrawLine(Vector2 start, float length, float angle, Color color, float thickness = 1,
        float layerDepth = 0);
    void FillRectangle(RectangleF rectangle, Color color, float layerDepth = 0);
    
    void DrawPolygon(Vector2 offset, IReadOnlyList<Vector2> points, Color color, float thickness = 1f,
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

    void DrawPolygonEdge(Vector2 point1, Vector2 point2, Color color, float thickness, float layerDepth);

    void DrawLine(Vector2 start, Vector2 end, Color color, float thickness = 1, float layerDepth = 0) {
        var length = Vector2.Distance(start, end);
        var angle = start.AngleBetween(end);
        DrawLine(start, length, angle, color, thickness, layerDepth);
    }

    void DrawVector(Vector2 origin, Vector2 vector, Color color, float thickness = 1, float layerDepth = 0) {
        DrawLine(origin, vector.Length(), vector.Angle(), color, thickness, layerDepth);
    }

    void DrawCircle(Vector2 center, float radius, int sides, Color color, float thickness = 1,
        float layerDepth = 0) =>
        DrawPolygon(center, CreateCircle(radius, sides), color, thickness, layerDepth);

    void DrawEllipse(Vector2 center, Vector2 radius, int sides, Color color, float thickness = 1,
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
