using System.Drawing;
using System.Numerics;
using Genjin.Breakout.Components;
using Genjin.Breakout.Systems;
using Genjin.Core.Entities;
using Genjin.Core.Primitives;
using Rectangle = Genjin.Core.Primitives.Rectangle;
using Size = Genjin.Core.Primitives.Size;

namespace Genjin.Breakout;

internal class GameScene : IScene {
    private const int RowCount = 10;
    private const int ColCount = 10;

    private readonly Size gameSize = new(800, 480);
    private readonly World world;

    public GameScene(World world, SharedState sharedState) {
        this.world = world;
        for (var row = 0; row < RowCount; row++) {
            for (var col = 0; col < ColCount; col++) {
                CreateBlock(row, col);
            }
        }

        var paddle = CreatePaddle();
        CreateBall(paddle);
        CreateWalls();
        sharedState.GameState = GameState.Initial;
    }

    private void CreateWalls() {
        var leftWall = world.CreateEntity();
        leftWall.AddComponent(new Body(new Rectangle(new Vector2(-gameSize.Width, 0), gameSize)));
        leftWall.AddComponent(new Collidable(CollisionType.Wall));

        var rightWall = world.CreateEntity();
        rightWall.AddComponent(new Body(new Rectangle(new Vector2(gameSize.Width, 0), gameSize)));
        rightWall.AddComponent(new Collidable(CollisionType.Wall));

        var topWall = world.CreateEntity();
        topWall.AddComponent(new Body(new Rectangle(new Vector2(0, -gameSize.Height), gameSize)));
        topWall.AddComponent(new Collidable(CollisionType.Wall));

        var bottomWall = world.CreateEntity();
        bottomWall.AddComponent(new Body(new Rectangle(new Vector2(0, gameSize.Height), gameSize)));
        bottomWall.AddComponent(new Collidable(CollisionType.Wall));
    }

    private Entity CreatePaddle() {
        var paddleSize = new Size(100, 20);
        var paddle = world.CreateEntity();
        paddle.AddComponent(new Collidable(CollisionType.Paddle));
        paddle.AddComponent(new Body(new Rectangle(
            new Vector2((gameSize.Width - paddleSize.Width) / 2, gameSize.Height - 40),
            paddleSize)));
        paddle.AddComponent(new Colored(Color.Red));
        paddle.AddComponent(new Controllable());
        paddle.AddComponent(new Movable());
        return paddle;
    }

    private void CreateBall(Entity paddle) {
        var ball = world.CreateEntity();
        ball.AddComponent(new Collidable(CollisionType.Ball));
        ball.AddComponent(new Body(new Circle(paddle.GetComponent<Body>().Center.AddY(-20), 10)));
        ball.AddComponent(new Colored(Color.White));
        ball.AddComponent(new Movable());
        ball.AddComponent(new Ball());
    }

    private void CreateBlock(int row, int col) {
        var block = world.CreateEntity();
        var size = new Size(40, 20);
        const int padding = 2;
        var offset = new Vector2((gameSize.Width - (RowCount * size.Width) - (padding * ColCount)) / 2, 50);

        block.AddComponent(new Body(new Rectangle(
            new Vector2(col * (size.Width + padding), row * (size.Height + padding)) + offset,
            size)));
        block.AddComponent(new Colored(Color.Gold));
        block.AddComponent(new Collidable(CollisionType.Wall));
    }
}

internal record Ball;
