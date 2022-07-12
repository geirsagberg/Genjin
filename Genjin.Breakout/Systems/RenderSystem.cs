using System.Drawing;
using Genjin.Breakout.Components;
using Genjin.Core;
using Genjin.Core.Entities;
using Genjin.Core.Primitives;
using RectangleF = Genjin.Core.Primitives.RectangleF;

namespace Genjin.Breakout;

internal class RenderSystem : IDrawSystem {
    private readonly ShapeRenderer shapeRenderer;
    private readonly World world;

    public RenderSystem(ShapeRenderer shapeRenderer, World world) {
        this.shapeRenderer = shapeRenderer;
        this.world = world;
    }

    public void Draw() {
        foreach (var entity in world.GetEntitiesMatchingAll(typeof(Transform), typeof(Colored))) {
            var transform = entity.Get<Transform>();
            var colored = entity.Get<Colored>();

            shapeRenderer.FillRectangle(
                new RectangleF(transform.Position.X, transform.Position.Y, transform.Size.Width, transform.Size.Height),
                colored.Color);
        }

        foreach (var entity in world.GetEntitiesMatchingAll(typeof(GameBounds))) {
            var gameBounds = entity.Get<GameBounds>();

            shapeRenderer.DrawRectangle(new RectangleF(Point2F.Zero, gameBounds.Size),
                Color.Blue);
        }
    }
}
