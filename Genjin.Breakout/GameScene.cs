using System.Drawing;
using System.Numerics;
using Genjin.Breakout.Components;
using Genjin.Breakout.Systems;
using Genjin.Core;
using Genjin.Core.Entities;

namespace Genjin.Breakout;

internal class GameScene : IScene {
    private const int RowCount = 10;
    private const int ColCount = 10;
    private readonly World world;

    private readonly SizeF gameSize = new(800, 480);

    public GameScene(World world, SharedState sharedState) {
        this.world = world;
        for (var row = 0; row < RowCount; row++) {
            for (var col = 0; col < ColCount; col++) {
                CreateBlock(row, col);
            }
        }

        var paddle = CreatePaddle();
        CreateBall(paddle);
        sharedState.GameState = GameState.Initial;
        sharedState.GameSize = gameSize;
    }

    private void CreateBall(Entity paddle) {
        var ball = world.CreateEntity();
        ball.Add(new Collidable(CollisionType.Ball));
        ball.Add(new Transform(paddle.Get<Transform>().Position + new Vector2(40, -20), 0, new SizeF(20f, 20f)));
        ball.Add(new Colored(Color.White));
        ball.Add(new Movable());
        ball.Add(new Ball());
    }

    private Entity CreatePaddle() {
        var paddleSize = new Size(100, 20);
        var paddle = world.CreateEntity();
        paddle.Add(new Collidable(CollisionType.Wall));
        paddle.Add(new Transform(new Vector2((gameSize.Width - paddleSize.Width) / 2, gameSize.Height - 40), 0,
            paddleSize));
        paddle.Add(new Colored(Color.Red));
        paddle.Add(new Controllable());
        paddle.Add(new Movable());
        return paddle;
    }

    private void CreateBlock(int row, int col) {
        var block = world.CreateEntity();
        var size = new Size(40, 20);
        const int padding = 2;
        var offset = new Vector2((gameSize.Width - (RowCount * size.Width) - (padding * ColCount)) / 2, 50);

        block.Add(new Transform(new Vector2(col * (size.Width + padding), row * (size.Height + padding)) + offset, 0,
            size));
        block.Add(new Colored(Color.Gold));
    }
}

internal record Ball;
