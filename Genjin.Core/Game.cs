using System.Diagnostics;
using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace Genjin.Core;

public class Game : IDisposable
{
    private bool running = true;
    protected Sdl2Window Window { get; }
    protected GraphicsDevice GraphicsDevice { get; }
    protected CommandList CommandList { get; }
    protected DeviceBuffer VertexBuffer { get; }
    protected DeviceBuffer IndexBuffer { get; }
    protected Pipeline Pipeline { get; }

    public Game()
    {
        var windowCreateInfo = new WindowCreateInfo(100, 100, 1024, 768, WindowState.Normal, "Game");
        Window = VeldridStartup.CreateWindow(ref windowCreateInfo);
        var options = new GraphicsDeviceOptions {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true,
        };

        GraphicsDevice = VeldridStartup.CreateGraphicsDevice(Window, options);

        Window.Resized += () => GraphicsDevice.ResizeMainWindow((uint)Window.Width, (uint)Window.Height);

        var resourceFactory = GraphicsDevice.ResourceFactory;


        VertexBuffer = resourceFactory.CreateBuffer(
            new BufferDescription(4 * VertexPositionColor.SizeInBytes,
                BufferUsage.VertexBuffer));
        IndexBuffer =
            resourceFactory.CreateBuffer(new BufferDescription(4 * sizeof(ushort),
                BufferUsage.IndexBuffer));
        var vertexLayout = new VertexLayoutDescription(
            new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate,
                VertexElementFormat.Float2),
            new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));

        var pipelineDescription = new GraphicsPipelineDescription(
            BlendStateDescription.SingleOverrideBlend,
            new DepthStencilStateDescription(true, true, ComparisonKind.LessEqual),
            new RasterizerStateDescription(FaceCullMode.Back, PolygonFillMode.Solid, FrontFace.Clockwise, true, false),
            PrimitiveTopology.TriangleStrip,
            new ShaderSetDescription(new[] { vertexLayout }, Shaders),
            Array.Empty<ResourceLayout>(),
            GraphicsDevice.SwapchainFramebuffer.OutputDescription
        );
        Pipeline = resourceFactory.CreateGraphicsPipeline(pipelineDescription);
        CommandList = resourceFactory.CreateCommandList();
    }

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
        CommandList.SetVertexBuffer(0, VertexBuffer);
        CommandList.SetIndexBuffer(IndexBuffer, IndexFormat.UInt16);
        CommandList.SetPipeline(Pipeline);
        CommandList.DrawIndexed(4, 1, 0, 0, 0);
        CommandList.End();
        GraphicsDevice.SubmitCommands(CommandList);
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
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
            Pipeline.Dispose();
            foreach (var shader in Shaders) {
                shader.Dispose();
            }
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

public record struct VertexPositionColor(Vector2 Position, RgbaFloat Color)
{
    public const uint SizeInBytes = 24;
}