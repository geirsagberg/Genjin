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
    private TextRenderer TextRenderer { get; }
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

        GraphicsDevice = VeldridStartup.CreateGraphicsDevice(Window, options, GraphicsBackend.Vulkan);

        Window.Resized += OnWindowOnResized;

        var shaders = VeldridSpriteBatch.LoadDefaultShaders(GraphicsDevice);
        CommandList = ResourceFactory.CreateCommandList();

        Fence = ResourceFactory.CreateFence(false);

        SpriteBatch = new VeldridSpriteBatch(GraphicsDevice, GraphicsDevice.SwapchainFramebuffer.OutputDescription,
            shaders);
        TextRenderer = new TextRenderer(GraphicsDevice, SpriteBatch);
    }

    private void OnWindowOnResized()
        => GraphicsDevice.ResizeMainWindow((uint)Window.Width, (uint)Window.Height);

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

    public async Task Start()
    {
        Init();

        // Can we live without real time?


        // 1 physics update -must- take shorter real time than physics time
        // if not? do 0-1 physics update + 1 draw (?) 


        // Physics: |----------|----------|----------|
        // Draw:    |------|------|------|------|------|

        // Scenario 1: Drawing takes shorter than target update interval

        // Scenario 2: Drawing takes longer than target update interval
        // - Physics clock and game must slow down


        // New frame!
        // How much time has passed?
        // - Less than 1 update interval
        //   - Do not update, just draw again
        // - More than 1 update interval
        //   - Update 


        var realTime = Stopwatch.StartNew();

        var physicsTime = TimeSpan.Zero;

        while (running && Window.Exists) {
            physicsTime = await GameLoop(realTime, physicsTime);
        }
    }

    private async Task<TimeSpan> GameLoop(Stopwatch realTime, TimeSpan physicsTime)
    {
        var physicsFrequency = 10;
        var physicsInterval = TimeSpan.FromSeconds(1.0 / physicsFrequency);
        var maxFrameSkip = 5;

        var frameStart = realTime.Elapsed;

        var framesSkipped = 0;
        while (physicsTime < frameStart
               && framesSkipped < maxFrameSkip
              ) {
            await Update(physicsInterval);
            physicsTime += physicsInterval;
            framesSkipped++;
        }

        // await Task.Delay(100);
        // Thread.Sleep(100);

        Window.PumpEvents();

        Draw(realTime);

        return physicsTime;
    }

    private void CreateResources()
    {
        // throw new NotImplementedException();
    }

    protected void Stop() => running = false;

    protected abstract void DrawSprites(VeldridSpriteBatch spriteBatch, TextRenderer textRenderer, Stopwatch realTime);

    protected virtual void Draw(Stopwatch realTime)
    {
        CommandList.Begin();
        CommandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
        CommandList.ClearColorTarget(0, RgbaFloat.Black);

        SpriteBatch.Begin();
        SpriteBatch.ViewMatrix = Matrix4x4.CreateOrthographicOffCenter(0, Window.Width, 0, Window.Height, -10, 10);
        DrawSprites(SpriteBatch, TextRenderer, realTime);
        SpriteBatch.DrawBatch(CommandList);
        SpriteBatch.End();

        CommandList.End();

        Fence.Reset();
        GraphicsDevice.SubmitCommands(CommandList, Fence);
        GraphicsDevice.WaitForFence(Fence);
        GraphicsDevice.SwapBuffers();
    }

    protected abstract Task Update(TimeSpan interval);
}