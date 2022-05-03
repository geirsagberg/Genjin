using System.Drawing;
using Genjin.Breakout.Components;
using Genjin.Core;
using Genjin.Example;

namespace Genjin.Breakout;

internal class RenderSystem : IDrawSystem {
    private readonly ShapeRenderer shapeRenderer;
    private readonly World world;

    public RenderSystem(ShapeRenderer shapeRenderer, World world) {
        this.shapeRenderer = shapeRenderer;
        this.world = world;
    }

    public void Draw() {
        foreach (var entity in world.GetEntitiesMatchingAll(typeof(Transform), typeof(Color))) {
            var transform = entity.Get<Transform>();
            var color = entity.Get<Color>();

            shapeRenderer.FillRectangle(
                new RectangleF(transform.Position.X, transform.Position.Y, transform.Size.Width, transform.Size.Height),
                color);
        }
    }
}
