﻿using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Peridot.Veldrid;
using StbImageSharp;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Rectangle = System.Drawing.Rectangle;

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
    private TextureWrapper WhitePixelTexture { get; }
    protected ResourceFactory ResourceFactory => GraphicsDevice.ResourceFactory;

    protected Game()
    {
        Window = new Sdl2Window("Game", 100, 100, 1024, 768, SDL_WindowFlags.Resizable | SDL_WindowFlags.Shown,
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
        var options = new GraphicsDeviceOptions {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true,
            SyncToVerticalBlank = true,
            Debug = true,
        };

        GraphicsDevice = VeldridStartup.CreateGraphicsDevice(Window, options, GraphicsBackend.Vulkan);

        Window.Resized += OnWindowOnResized;

        var shaders = VeldridSpriteBatch.LoadDefaultShaders(GraphicsDevice);
        CommandList = ResourceFactory.CreateCommandList();

        Fence = ResourceFactory.CreateFence(false);

        SpriteBatch = new VeldridSpriteBatch(GraphicsDevice, GraphicsDevice.SwapchainFramebuffer.OutputDescription,
            shaders);
        TextRenderer = new TextRenderer(GraphicsDevice, SpriteBatch);

        WhitePixelTexture = CreateWhitePixelTexture();
    }

    private TextureWrapper CreateWhitePixelTexture()
    {
        var textureDescription = new TextureDescription(1, 1, 1, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.Sampled, TextureType.Texture2D);
        var texture = ResourceFactory.CreateTexture(textureDescription);
        GraphicsDevice.UpdateTexture(texture, new byte[] { 255, 255, 255, 255 }, 0, 0, 0, 1, 1, 1, 0, 0);
        return new TextureWrapper(texture);
    }

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

    protected abstract void Init();

    protected void FillRectangle(Rectangle rectangle, Color color)
    {
        SpriteBatch.Draw(WhitePixelTexture, rectangle, new Rectangle(0, 0, 1, 1), color, 0,
            Vector2.Zero,
            0);
    }

    public async Task Start()
    {
        Init();

        var realTime = Stopwatch.StartNew();

        var physicsTime = TimeSpan.Zero;

        while (running && Window.Exists) {
            physicsTime = await GameLoop(realTime, physicsTime);
        }
    }

    private async Task<TimeSpan> GameLoop(Stopwatch realTime, TimeSpan physicsTime)
    {
        var input = Window.PumpEvents();

        await UpdateBasedOnInput(input);

        var physicsFrequency = 60;
        var physicsInterval = TimeSpan.FromSeconds(1.0 / physicsFrequency);
        var maxFrameSkip = 5;
        var framesSkipped = 0;
        while (physicsTime < realTime.Elapsed && framesSkipped < maxFrameSkip) {
            await UpdatePhysics(physicsInterval);
            physicsTime += physicsInterval;
            framesSkipped++;
        }

        var interpolation = (realTime.Elapsed + physicsInterval - physicsTime) / physicsInterval;

        Draw(realTime, interpolation, physicsInterval);

        // await Task.Delay(100);

        return physicsTime;
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
            SpriteBatch.ViewMatrix = Matrix4x4.CreateOrthographicOffCenter(0, Window.Width, 0, Window.Height, -10, 10);
            DrawSprites(realTime, interpolation, physicsInterval);
            SpriteBatch.DrawBatch(CommandList);
            SpriteBatch.End();

            CommandList.End();

            Fence.Reset();
            GraphicsDevice.SubmitCommands(CommandList, Fence);
            GraphicsDevice.WaitForFence(Fence);
            GraphicsDevice.SwapBuffers();
        }
    }

    protected abstract Task UpdatePhysics(TimeSpan physicsInterval);
}