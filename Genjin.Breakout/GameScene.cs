using System.Drawing;
using System.Numerics;
using Genjin.Breakout.Components;
using Genjin.Core;
using Genjin.Example;

namespace Genjin.Breakout;

internal class GameScene : IScene {
    private const int rowCount = 10;
    private const int colCount = 10;
    private readonly World world;
    private readonly ShapeRenderer shapeRenderer;

    private readonly SizeF gameSize = new(800, 480);

    public GameScene(World world, ShapeRenderer shapeRenderer) {
        this.world = world;
        this.shapeRenderer = shapeRenderer;
        for (var row = 0; row < rowCount; row++) {
            for (var col = 0; col < colCount; col++) {
                CreateBlock(row, col);
            }
        }
    }

    private void CreateBlock(int row, int col) {
        var block = world.CreateEntity();
        var size = new Size(40, 20);
        const int padding = 2;
        var offset = new Vector2((gameSize.Width - (rowCount * size.Width) - (padding * colCount)) / 2, 50);

        block.Add(new Transform(new Vector2(col * (size.Width + padding), row * (size.Height + padding)) + offset, 0,
            size));
        block.Add(Color.Gold);
    }

    public void Draw() =>
        shapeRenderer.DrawRectangle(new RectangleF(0, 0, gameSize.Width, gameSize.Height), Color.Blue);
}
