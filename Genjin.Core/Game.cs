using System.Diagnostics;
using System.Numerics;
using Peridot.Veldrid;
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

    public Game()
    {
        var windowCreateInfo = new WindowCreateInfo(100, 100, 1024, 768, WindowState.Normal, "Game");
        Window = VeldridStartup.CreateWindow(windowCreateInfo);
        var options = new GraphicsDeviceOptions {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true,
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

    public async Task Start()
    {
        var stopwatch = Stopwatch.StartNew();
        var elapsed = new GameTime { Elapsed = stopwatch.Elapsed };
        while (running && Window.Exists) {
            Window.PumpEvents();
            Update(elapsed);
            Draw(elapsed);
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
        CommandList.ClearDepthStencil(0f);

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

    protected virtual void Update(GameTime gameTime)
    {
    }
}

public record struct GameTime
{
    public TimeSpan Elapsed;
}