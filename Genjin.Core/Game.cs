using System.Diagnostics;
using System.Numerics;
using Peridot.Veldrid;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Genjin.Core;

public class Game : IDisposable
{
    private bool running = true;
    protected Sdl2Window Window { get; }
    protected GraphicsDevice GraphicsDevice { get; }
    protected CommandList CommandList { get; }
    protected VeldridSpriteBatch SpriteBatch { get; }
    protected ResourceFactory ResourceFactory => GraphicsDevice.ResourceFactory;

    public Game()
    {
        var windowCreateInfo = new WindowCreateInfo(100, 100, 1024, 768, WindowState.Normal, "Game");
        Window = VeldridStartup.CreateWindow(ref windowCreateInfo);
        var options = new GraphicsDeviceOptions {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true,
        };

        GraphicsDevice = VeldridStartup.CreateGraphicsDevice(Window, options);

        Window.Resized += OnWindowOnResized;

        var resourceFactory = GraphicsDevice.ResourceFactory;

        CommandList = resourceFactory.CreateCommandList();

        var shaders = VeldridSpriteBatch.LoadDefaultShaders(GraphicsDevice);

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

    protected virtual void Draw(GameTime gameTime)
    {
        
        CommandList.Begin();
        CommandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
        CommandList.ClearColorTarget(0, RgbaFloat.Black);
        CommandList.End();
        GraphicsDevice.SubmitCommands(CommandList);4
        GraphicsDevice.SwapBuffers();
    }

    protected virtual void Update(GameTime gameTime)
    {
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing) {
            GraphicsDevice.Dispose();
            CommandList.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

public record struct GameTime
{
    public TimeSpan Elapsed;
}
