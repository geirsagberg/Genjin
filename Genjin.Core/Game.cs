using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Genjin.Example;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peridot.Veldrid;
using StbImageSharp;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Genjin.Core;

public abstract class Game {
    private readonly List<Simulation> simulations = new();

    private TimeSpan lastFrame = TimeSpan.Zero;
    private Stopwatch realTime = null!;
    private bool running = true;

    protected readonly MessageHub MessageHub = new();

    protected Game() {
        var gameSettings = LoadGenjinSettings();
        Window = CreateWindow("Game", gameSettings);
        Window.Resized += OnWindowOnResized;
        var options = GetGraphicsDeviceOptions(gameSettings);
        GraphicsDevice = VeldridStartup.CreateGraphicsDevice(Window, options, GraphicsBackend.Vulkan);
        var shaders = VeldridSpriteBatch.LoadDefaultShaders(GraphicsDevice);
        CommandList = ResourceFactory.CreateCommandList();
        Fence = ResourceFactory.CreateFence(false);
        SpriteBatch = new VeldridSpriteBatch(GraphicsDevice, GraphicsDevice.SwapchainFramebuffer.OutputDescription,
            shaders);
        TextRenderer = new TextRenderer(GraphicsDevice, SpriteBatch);
        ShapeRenderer = new ShapeRenderer(GraphicsDevice, SpriteBatch);
        GuiRenderer = new ImGuiRenderer(GraphicsDevice, GraphicsDevice.SwapchainFramebuffer.OutputDescription,
            gameSettings.Width, gameSettings.Height);
        DefaultFont = LoadFont("Assets/Fonts/arial.ttf");

        MessageHub.Subscribe<StopMessage>(delegate { Stop(); });
    }

    private Fence Fence { get; }

    protected Font DefaultFont { get; }

    protected Sdl2Window Window { get; }
    private GraphicsDevice GraphicsDevice { get; }
    private CommandList CommandList { get; }
    protected VeldridSpriteBatch SpriteBatch { get; }
    protected TextRenderer TextRenderer { get; }
    protected ShapeRenderer ShapeRenderer { get; }
    protected ImGuiRenderer GuiRenderer { get; }
    protected ResourceFactory ResourceFactory => GraphicsDevice.ResourceFactory;

    protected IServiceProvider Services { get; private set; } = null!;

    private static Sdl2Window CreateWindow(string title, GenjinSettings gameSettings) =>
        new(title, 100, 100, gameSettings.Width, gameSettings.Height, GetWindowFlags(gameSettings),
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

    private static GraphicsDeviceOptions GetGraphicsDeviceOptions(GenjinSettings gameSettings) =>
        new() {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true,
            SyncToVerticalBlank = gameSettings.VSync
        };

    private static SDL_WindowFlags GetWindowFlags(GenjinSettings gameSettings) =>
        SDL_WindowFlags.Resizable |
        SDL_WindowFlags.Shown |
        gameSettings.Display switch {
            DisplayMode.Fullscreen => SDL_WindowFlags.Fullscreen,
            DisplayMode.Borderless => SDL_WindowFlags.FullScreenDesktop,
            _ => 0
        };

    private static GenjinSettings LoadGenjinSettings() =>
        new ConfigurationBuilder()
            .AddIniFile("defaultConfig.ini", false)
            .AddIniFile("config.ini", true, true)
            .Build()
            .GetSection("Genjin").Get<GenjinSettings>();

    private void OnWindowOnResized() {
        lock (Window) {
            GraphicsDevice.ResizeMainWindow((uint) Window.Width, (uint) Window.Height);
            GuiRenderer.WindowResized(Window.Width, Window.Height);
        }
    }

    protected TextureWrapper LoadTexture(string path) {
        var bytes = File.ReadAllBytes(path);
        var image = ImageResult.FromMemory(bytes);
        Debug.Assert(image != null);
        var textureDescription = new TextureDescription(
            (uint) image.Width, (uint) image.Height,
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

    protected static Font LoadFont(string path) {
        var bytes = File.ReadAllBytes(path);
        var font = new Font();
        font.AddFont(bytes);
        return font;
    }

    protected abstract Task Init();

    protected virtual void ConfigureServices(IServiceCollection services) { }

    public async Task Start() {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        await Init();

        realTime = Stopwatch.StartNew();

        while (running && Window.Exists) {
            await GameLoop();
        }
    }

    private async ValueTask GameLoop() {
        var thisFrame = realTime.Elapsed;
        var input = Window.PumpEvents();
        var deltaTime = thisFrame - lastFrame;

        GuiRenderer.Update((float) deltaTime.TotalSeconds, input);

        foreach (var simulation in simulations) {
            await simulation.Update(thisFrame);
        }

        DrawInternal(deltaTime);

        lastFrame = thisFrame;
    }

    protected void Stop() => running = false;

    protected abstract void Draw(TimeSpan sincePreviousFrame);

    private void DrawInternal(TimeSpan sincePreviousFrame) {
        lock (Window) {
            CommandList.Begin();
            CommandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
            CommandList.ClearColorTarget(0, RgbaFloat.Black);

            SpriteBatch.Begin();
            SpriteBatch.ViewMatrix =
                Matrix4x4.CreateOrthographicOffCenter(0, Window.Width, 0, Window.Height, -10, 10);
            Draw(sincePreviousFrame);
            SpriteBatch.DrawBatch(CommandList);
            SpriteBatch.End();

            GuiRenderer.Render(GraphicsDevice, CommandList);

            CommandList.End();

            Fence.Reset();
            GraphicsDevice.SubmitCommands(CommandList, Fence);
            GraphicsDevice.WaitForFence(Fence);
            if (Window.Exists) {
                GraphicsDevice.SwapBuffers();
            }
        }
    }

    protected Simulation StartSimulation(ushort updatesPerSecond = 60,
        ushort maxSkippedUpdates = 5) {
        var simulation = new Simulation(TimeSpan.Zero, updatesPerSecond, maxSkippedUpdates);
        simulations.Add(simulation);
        return simulation;
    }

    protected void DrawString(string text, Vector2 position) =>
        TextRenderer.DrawString(DefaultFont, 32, text, position, Color.Aqua, 0f, Vector2.Zero,
            Vector2.One, 0f);
}

public record struct StopMessage : INotification;
