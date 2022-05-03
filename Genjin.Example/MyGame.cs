﻿using System.Numerics;
using Genjin.Core;
using Peridot;
using Peridot.Veldrid;
using Veldrid;

namespace Genjin.Example;

internal class MyGame : Game {
    private const double FpsSmoothing = 0.9;

    private readonly Dictionary<Key, GameKey> keyMap = new() {
        { Key.A, GameKey.Left },
        { Key.W, GameKey.Up },
        { Key.S, GameKey.Down },
        { Key.D, GameKey.Right }
    };

    private readonly HashSet<GameKey> pressedKeys = new();
    private AnimatedSprite<PlayerState> animatedSprite = null!;

    private TimeSpan animationTime;

    private int currentAnimationFrame;

    private PlayerState currentPlayerState;

    private int draws;

    private Facing facing = Facing.Right;

    private double fps;
    private Vector2 mousePosition;
    private TextureWrapper playerSprite;
    private Simulation simulation = null!;

    private SpriteSheet spriteSheet = null!;
    private Transform transform;
    private int updates;

    private Vector2 velocity = Vector2.Zero;

    protected override Task Init() {
        playerSprite = LoadTexture("Assets/Sprites/player.png");

        spriteSheet = new SpriteSheet(playerSprite, 21, 1);
        animatedSprite = new AnimatedSprite<PlayerState>(spriteSheet, new Dictionary<PlayerState, Animation> {
            { PlayerState.Idle, new(..0) },
            { PlayerState.Running, new(1..9) }
        });

        transform = new Transform(new Vector2(100, 100), default, spriteSheet.SpriteSize);

        simulation = StartSimulation(UpdatePhysics);

        return Task.CompletedTask;
    }

    protected override Task UpdateBasedOnInput(InputSnapshot input) {
        UpdatePressedKeys(input);

        var newVelocity =
            new Vector2((pressedKeys.Contains(GameKey.Left) ? -1f : 0) + (pressedKeys.Contains(GameKey.Right) ? 1f : 0),
                (pressedKeys.Contains(GameKey.Up) ? -1f : 0) + (pressedKeys.Contains(GameKey.Down) ? 1f : 0));

        velocity = newVelocity.NormalizeOrZero();

        facing = velocity.X switch {
            > 0 => Facing.Right,
            < 0 => Facing.Left,
            _ => facing
        };

        mousePosition = input.MousePosition;

        return Task.CompletedTask;
    }

    private void UpdatePressedKeys(InputSnapshot input) {
        foreach (var keyEvent in input.KeyEvents) {
            if (keyMap.TryGetValue(keyEvent.Key, out var key)) {
                if (keyEvent.Down) {
                    pressedKeys.Add(key);
                } else {
                    pressedKeys.Remove(key);
                }
            }
        }
    }

    protected override void Draw(TimeSpan sincePreviousFrame) {
        var currentFps = 1.0 / sincePreviousFrame.TotalSeconds;
        fps = (fps * FpsSmoothing) + (currentFps * (1.0 - FpsSmoothing));

        animationTime += sincePreviousFrame;

        var newPlayerState = pressedKeys.Any() ? PlayerState.Running : PlayerState.Idle;

        var animation = animatedSprite.Animations[newPlayerState];

        if (newPlayerState == PlayerState.Running) {
            if (currentPlayerState != PlayerState.Running) {
                currentAnimationFrame = 0;
            }

            currentPlayerState = newPlayerState;
            var animationFrameInterval = TimeSpan.FromSeconds(1 / animation.Fps);
            while (animationTime > animationFrameInterval) {
                currentAnimationFrame++;
                animationTime -= animationFrameInterval;
            }
        }

        currentAnimationFrame %= animation.Frames.GetLength();

        var viewPosition = transform.Position +
            (velocity * GetFactor(simulation.UpdateInterval) * simulation.CurrentInterpolation);

        var spriteSheetIndex = animation.Frames.Start.Value + currentAnimationFrame;

        SpriteBatch.DrawSprite(spriteSheet, spriteSheetIndex,
            transform with {
                Position = viewPosition
            }, facing == Facing.Right ? SpriteOptions.None : SpriteOptions.FlipHorizontally);

        DrawString($"{transform.Position.X:F2}", Vector2.Zero);
        DrawString($"{transform.Position.Y:F2}", new Vector2(150, 0));
        DrawString($"Draws: {draws++}", new Vector2(0, 40));
        DrawString($"FPS: {fps:F0}", new Vector2(200, 40));
        DrawString($"Updates: {updates}", new Vector2(0, 80));
    }

    protected ValueTask UpdatePhysics(TimeSpan physicsInterval) {
        var newPosition = transform.Position + (velocity * GetFactor(physicsInterval));
        transform = transform with {
            Position = newPosition.Wrap(Window.Bounds.Size)
        };
        updates++;

        return ValueTask.CompletedTask;
    }

    private static float GetFactor(TimeSpan interval) => (float) interval.TotalSeconds * 200;

    private enum GameKey : byte {
        Up,
        Down,
        Left,
        Right
    }
}
