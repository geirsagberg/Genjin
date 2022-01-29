using System.Diagnostics;
using System.Numerics;
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
    private VeldridSpriteBatch SpriteBatch { get; }
    private Fence Fence { get; }
    protected ResourceFactory ResourceFactory => GraphicsDevice.ResourceFactory;

    protected Game()
    {
        var windowCreateInfo = new WindowCreateInfo(100, 100, 1024, 768, WindowState.Normal, "Game");
        Window = VeldridStartup.CreateWindow(windowCreateInfo);
        var options = new GraphicsDeviceOptions {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true,
            SyncToVerticalBlank = true,
            Debug = true,
        };

        GraphicsDevice = VeldridStartup.CreateGraphicsDevice(Window, options);

        Window.Resized += OnWindowOnResized;

        var shaders = VeldridSpriteBatch.LoadDefaultShaders(GraphicsDevice);
        CommandList = ResourceFactory.CreateCommandList();

        Fence = ResourceFactory.CreateFence(false);

        SpriteBatch = new VeldridSpriteBatch(GraphicsDevice, GraphicsDevice.SwapchainFramebuffer.OutputDescription,
            shaders);
    }

    private void OnWindowOnResized()
        => GraphicsDevice.ResizeMainWindow((uint)Window.Width, (uint)Window.Height);

    protected async Task<TextureWrapper> LoadTexture(string path)
    {
        var bytes = await File.ReadAllBytesAsync(path);
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

    protected abstract Task Init();

    public async Task Start()
    {
        await Init();
        var stopwatch = Stopwatch.StartNew();
        var elapsed = new GameTime { Elapsed = stopwatch.Elapsed };
        var fps = 60;
        var secsPerFrame = 1.0 / fps;
        var timespanPerFrame = TimeSpan.FromSeconds(secsPerFrame);
        while (running && Window.Exists) {
            Window.PumpEvents();
            Update(elapsed);
            Draw(elapsed);
            var timeLeft = stopwatch.Elapsed - elapsed.Elapsed + timespanPerFrame;
            if (timeLeft > TimeSpan.Zero) {
                await Task.Delay(timeLeft);
            }

            elapsed = elapsed with { Elapsed = stopwatch.Elapsed };
        }
    }

    private void CreateResources()
    {
        // throw new NotImplementedException();
    }

    protected void Stop() => running = false;

    protected abstract void DrawSprites(GameTime gameTime, VeldridSpriteBatch spriteBatch);

    protected virtual void Draw(GameTime gameTime)
    {
        CommandList.Begin();
        CommandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
        CommandList.ClearColorTarget(0, RgbaFloat.Black);

        SpriteBatch.Begin();
        SpriteBatch.ViewMatrix = Matrix4x4.CreateOrthographic(Window.Width, Window.Height, 0.01f, -100f);
        DrawSprites(gameTime, SpriteBatch);
        SpriteBatch.DrawBatch(CommandList);
        SpriteBatch.End();

        CommandList.End();

        Fence.Reset();
        GraphicsDevice.SubmitCommands(CommandList, Fence);
        GraphicsDevice.WaitForFence(Fence);
        GraphicsDevice.SwapBuffers();
    }

    protected abstract void Update(GameTime gameTime);
}

public record struct GameTime
{
    public TimeSpan Elapsed;
    public double ElapsedMilliseconds => Elapsed.TotalMilliseconds;
}