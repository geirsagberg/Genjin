using Genjin.Breakout.Components;
using Genjin.Core;
using Genjin.Core.Entities;
using Genjin.Core.Primitives;

namespace Genjin.Breakout;

internal class RenderSystem : IDrawSystem {
    private readonly IShapeRenderer shapeRenderer;
    private readonly World world;

    public RenderSystem(IShapeRenderer shapeRenderer, World world) {
        this.shapeRenderer = shapeRenderer;
        this.world = world;
    }

    public void Draw() {
        foreach (var entity in world.GetEntitiesMatchingAll(typeof(Body), typeof(Colored))) {
            var shape = entity.GetComponent<Body>().Shape;
            var colored = entity.GetComponent<Colored>();

            switch (shape) {
                case Rectangle rectangle:
                    shapeRenderer.FillRectangle(rectangle, colored.Color);
                    break;
                case Circle circle:
                    shapeRenderer.DrawCircle(circle.Center, circle.Radius, 16, colored.Color);
                    break;
            }
        }
    }
}
