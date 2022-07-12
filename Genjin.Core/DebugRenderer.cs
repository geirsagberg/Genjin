using System.Drawing;
using System.Numerics;
using Genjin.Example;
using RectangleF = Genjin.Core.Primitives.RectangleF;

namespace Genjin.Core;

public class DebugRenderer : IDebugRenderer {
    public readonly record struct PointDrawing(Vector2 Position, Color Color, float Size, float LayerDepth);
    public readonly record struct RectangleDrawing(RectangleF Rectangle, Color Color, float Thickness, float LayerDepth);
    public readonly record struct LineDrawing(Vector2 Start, float Length, float Angle, Color Color, float Thickness, float LayerDepth);
    public readonly record struct PolygonEdgeDrawing(Vector2 Point1, Vector2 Point2, Color Color, float Thickness, float LayerDepth);
    
    public DebugRenderer(MessageHub messageHub) {
        messageHub.Handle<ResetDebugRendererRequest>(OnResetDebugRenderer);
    }

    private void OnResetDebugRenderer(ResetDebugRendererRequest _) {
        Reset();
    }

    private void Reset() {
        Points.Clear();
        Rectangles.Clear();
        Lines.Clear();
        PolygonEdges.Clear();
        FilledRectangles.Clear();
    }
    
    public List<PointDrawing> Points { get; } = new();
    public List<RectangleDrawing> Rectangles { get; } = new();
    public List<LineDrawing> Lines { get; } = new();
    public List<PolygonEdgeDrawing> PolygonEdges { get; } = new();
    public List<RectangleDrawing> FilledRectangles { get; } = new();
    
    public void DrawPoint(Vector2 position, Color color, float size = 1, float layerDepth = 0) => Points.Add(new PointDrawing(position, color, size, layerDepth));

    public void DrawRectangle(RectangleF rectangle, Color color, float thickness = 1, float layerDepth = 0) => Rectangles.Add(new RectangleDrawing(rectangle, color, thickness, layerDepth));

    public void DrawLine(Vector2 start, float length, float angle, Color color, float thickness = 1, float layerDepth = 0) => Lines.Add(new LineDrawing(start, length, angle, color, thickness, layerDepth));

    public void FillRectangle(RectangleF rectangle, Color color, float layerDepth = 0) => Rectangles.Add(new RectangleDrawing(rectangle, color, 0, layerDepth));

    public void DrawPolygonEdge(Vector2 point1, Vector2 point2, Color color, float thickness, float layerDepth) => PolygonEdges.Add(new PolygonEdgeDrawing(point1, point2, color, thickness, layerDepth));
}
