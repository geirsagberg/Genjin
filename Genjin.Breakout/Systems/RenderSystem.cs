using Genjin.Breakout.Components;
using Genjin.Core;
using Genjin.Core.Entities;
using Genjin.Core.Primitives;

namespace Genjin.Breakout;

internal class RenderSystem : IDrawSystem {
    private readonly ShapeRenderer shapeRenderer;
    private readonly World world;

    public RenderSystem(ShapeRenderer shapeRenderer, World world) {
        this.shapeRenderer = shapeRenderer;
        this.world = world;
    }

    public void Draw() {
        foreach (var entity in world.GetEntitiesMatchingAll(typeof(Body), typeof(Colored))) {
            var transform = entity.GetComponent<Body>();
            var colored = entity.GetComponent<Colored>();

            shapeRenderer.FillRectangle(
                new RectangleF(transform.Position.X, transform.Position.Y, transform.Size.Width, transform.Size.Height),
                colored.Color);
        }
    }
}
