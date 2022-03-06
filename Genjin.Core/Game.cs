﻿using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Peridot.Veldrid;
using StbImageSharp;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Genjin.Core;

public abstract class Game
{
    private bool running = true;
    protected Sdl2Window Window { get; }
    private GraphicsDevice GraphicsDevice { get; }
    private CommandList CommandList { get; }
    protected VeldridSpriteBatch SpriteBatch { get; }
    protected TextRenderer TextRenderer { get; }
    private Fence Fence { get; }
    protected ShapeBatch ShapeRenderer { get; }
    protected ResourceFactory ResourceFactory => GraphicsDevice.ResourceFactory;

    protected Game(string title = "Game")
    {
        var gameSettings = LoadGenjinSettings();

        Window = new Sdl2Window(title, 100, 100, gameSettings.Width, gameSettings.Height, SDL_WindowFlags.Resizable |
            SDL_WindowFlags.Shown |
            gameSettings.Display switch {
                DisplayMode.Fullscreen => SDL_WindowFlags.Fullscreen,
                DisplayMode.Borderless => SDL_WindowFlags.FullScreenDesktop,
                _ => 0
            },
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
        var options = new GraphicsDeviceOptions {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true,
            SyncToVerticalBlank = gameSettings.VSync
        };

        GraphicsDevice = VeldridStartup.CreateGraphicsDevice(Window, options, GraphicsBackend.Vulkan);

        Window.Resized += OnWindowOnResized;

        var shaders = VeldridSpriteBatch.LoadDefaultShaders(GraphicsDevice);
        CommandList = ResourceFactory.CreateCommandList();

        Fence = ResourceFactory.CreateFence(false);

        SpriteBatch = new VeldridSpriteBatch(GraphicsDevice, GraphicsDevice.SwapchainFramebuffer.OutputDescription,
            shaders);
        TextRenderer = new TextRenderer(GraphicsDevice, SpriteBatch);

        ShapeRenderer = new ShapeBatch(SpriteBatch, GraphicsDevice);
    }

    private static GenjinSettings LoadGenjinSettings() =>
        new ConfigurationBuilder()
            .AddIniFile("defaultConfig.ini", false)
            .AddIniFile("config.ini", true, true)
            .Build()
            .GetSection("Genjin").Get<GenjinSettings>();

    private void OnWindowOnResized()
    {
        lock (Window) {
            GraphicsDevice.ResizeMainWindow((uint)Window.Width, (uint)Window.Height);
        }
    }

    protected TextureWrapper LoadTexture(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var image = ImageResult.FromMemory(bytes);
        Debug.Assert(image != null);
        var textureDescription = new TextureDescription(
            (uint)image.Width, (uint)image.Height,
            1, 1, 1,
            PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.Sampled,
            TextureType.Texture2D
        );
        var texture = ResourceFactory.CreateTexture(textureDescription);
        GraphicsDevice.UpdateTexture(texture, image.Data, 0, 0, 0, textureDescription.Width, textureDescription.Height,
            textureDescription.Depth, 0, 0);

        return new TextureWrapper(texture);
    }

    protected static Font LoadFont(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var font = new Font();
        font.AddFont(bytes);
        return font;
    }

    protected abstract Task Init();

    public async Task Start()
    {
        await Init();

        var realTime = Stopwatch.StartNew();

        var elapsedPhysicsTime = TimeSpan.Zero;

        while (running && Window.Exists) {
            elapsedPhysicsTime = await GameLoop(realTime, elapsedPhysicsTime);
        }
    }

    private async Task<TimeSpan> GameLoop(Stopwatch realTime, TimeSpan elapsedPhysicsTime)
    {
        var input = Window.PumpEvents();

        await UpdateBasedOnInput(input);

        var physicsFrequency = 60;
        var physicsInterval = TimeSpan.FromSeconds(1.0 / physicsFrequency);
        var maxFrameSkip = 5;
        var framesSkipped = 0;
        while (elapsedPhysicsTime < realTime.Elapsed && framesSkipped < maxFrameSkip) {
            await UpdatePhysics(physicsInterval);
            elapsedPhysicsTime += physicsInterval;
            framesSkipped++;
        }

        var interpolation = (realTime.Elapsed + physicsInterval - elapsedPhysicsTime) / physicsInterval;

        Draw(realTime, interpolation, physicsInterval);

        return elapsedPhysicsTime;
    }

    protected abstract Task UpdateBasedOnInput(InputSnapshot input);

    protected void Stop() => running = false;

    protected abstract void DrawSprites(Stopwatch realTime, double interpolation, TimeSpan physicsInterval);

    protected virtual void Draw(Stopwatch realTime, double interpolation, TimeSpan physicsInterval)
    {
        lock (Window) {
            CommandList.Begin();
            CommandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
            CommandList.ClearColorTarget(0, RgbaFloat.Black);

            SpriteBatch.Begin();
            SpriteBatch.ViewMatrix =
                Matrix4x4.CreateOrthographicOffCenter(0, Window.Width, 0, Window.Height, -10, 10);
            DrawSprites(realTime, interpolation, physicsInterval);
            SpriteBatch.DrawBatch(CommandList);
            SpriteBatch.End();

            CommandList.End();

            Fence.Reset();
            GraphicsDevice.SubmitCommands(CommandList, Fence);
            GraphicsDevice.WaitForFence(Fence);
            if (Window.Exists) {
                GraphicsDevice.SwapBuffers();
            }
        }
    }

    protected abstract Task UpdatePhysics(TimeSpan physicsInterval);
}