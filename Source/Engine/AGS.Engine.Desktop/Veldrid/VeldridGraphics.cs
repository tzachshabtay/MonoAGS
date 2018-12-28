using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using AGS.API;
using Veldrid;
using Veldrid.Utilities;

namespace AGS.Engine.Desktop
{
    public class TextureContainer
    {
        public Texture Texture { get; set; }
        public ResourceSet WorldTextureSet { get; set; }
    }

	public class VeldridGraphics : IGraphicsBackend
    {
        private GraphicsDevice _graphicsDevice;
        private readonly Dictionary<int, TextureContainer> _textures = new Dictionary<int, TextureContainer>(1000);
        private readonly Dictionary<(int, BufferType), DeviceBuffer> _buffers = new Dictionary<(int, BufferType), DeviceBuffer>();
        private DisposeCollectorResourceFactory _factory;
        private Pipeline _pipeline;
        private ResourceLayout _projViewLayout, _worldTextureLayout;
        private int _boundTexture;
        private DeviceBuffer _currentVertexBuffer, _currentIndexBuffer, _worldBuffer, _projectionBuffer, _viewBuffer;
        private ResourceSet _projViewSet;
        private CommandList _cl;
        static int _lastTexture = 0;

        public void ActiveTexture(int paramIndex)
        {
        }

        public bool AreShadersSupported() => false;

        public void AttachShader(int programId, int shaderId)
        {
        }

        public void BindBuffer(int bufferId, BufferType bufferType)
        {
        }

        public void BindFrameBuffer(int frameBufferId)
        {
        }

        public void BindTexture2D(int textureId)
        {
            _boundTexture = textureId;
        }

        public void BufferData<TBufferItem>(TBufferItem[] items, uint itemSize, BufferType bufferType) where TBufferItem : struct
        {
            var buffer = _buffers.GetOrAdd(((int)itemSize, bufferType), () =>
                   _factory.CreateBuffer(new BufferDescription(itemSize, getBufferUsage(bufferType))));
            _graphicsDevice.UpdateBuffer(buffer, 0, items);
            switch (bufferType)
            {
                case BufferType.ArrayBuffer:
                    _currentVertexBuffer = buffer;
                    break;
                case BufferType.ElementArrayBuffer:
                    _currentIndexBuffer = buffer;
                    break;
            }
        }

        public void ClearColor(float r, float g, float b, float a)
        {
        }

        public void ClearScreen()
        {
        }

        public void CompileShader(int shaderId)
        {
        }

        public int CreateProgram()
        {
            return 0;
        }

        public int CreateShader(ShaderMode shaderType)
        {
            return 0;
        }

        public void DeleteFrameBuffer(int frameBufferId)
        {
        }

        public void DeleteProgram(int programId)
        {
        }

        public void DeleteShader(int shaderId)
        {
        }

        public void DeleteTexture(int textureId)
        {
        }

        public void DrawArrays(PrimitiveMode primitiveType, int first, int count)
        {
        }

        public void DrawElements(PrimitiveMode primitiveType, int count, short[] indices)
        {
            var worldTextureSet = _textures[_boundTexture].WorldTextureSet;
            _cl.Begin();

            /*var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
                1.0f,
                (float)Window.Width / Window.Height,
                0.5f, //-1?
                100f);*/
            var projectionMatrix = Matrix4x4.CreateOrthographic(GLUtils.CurrentGlobalResolution.Width, GLUtils.CurrentGlobalResolution.Height, -1f, 1f);
            _cl.UpdateBuffer(_projectionBuffer, 0, projectionMatrix); //1?

            //var viewMatrix = Matrix4x4.CreateLookAt(Vector3.UnitZ * 2.5f, Vector3.Zero, Vector3.UnitY);
            var viewMatrix = Matrix4x4.Identity;
            _cl.UpdateBuffer(_viewBuffer, 0, viewMatrix);
            var matrix = Matrix4x4.Identity;
            _cl.UpdateBuffer(_worldBuffer, 0, ref matrix);

            _cl.SetFramebuffer(_graphicsDevice.MainSwapchain.Framebuffer);
            _cl.ClearColorTarget(0, RgbaFloat.Black);
            _cl.ClearDepthStencil(1f);
            _cl.SetPipeline(_pipeline);
            _cl.SetVertexBuffer(0, _currentVertexBuffer);
            _cl.SetIndexBuffer(_currentIndexBuffer, IndexFormat.UInt16);
            _cl.SetGraphicsResourceSet(0, _projViewSet);
            _cl.SetGraphicsResourceSet(1, worldTextureSet);
            _cl.DrawIndexed(6, 1, 0, 0, 0);

            _cl.End();
            _graphicsDevice.SubmitCommands(_cl);
        }

        public bool DrawFrameBuffer()
        {
            return true;
        }

        public void FrameBufferTexture2D(int textureId)
        {
        }

        public int GenBuffer()
        {
            return 0;
        }

        public int GenFrameBuffer()
        {
            return 0;
        }

        public int GenTexture()
        {
            return _lastTexture++;
        }

        public int GetMaxTextureUnits()
        {
            return 100;
        }

        public string GetProgramInfoLog(int programId)
        {
            return "";
        }

        public int GetProgramLinkErrorCode(int programId)
        {
            return 0;
        }

        public int GetShaderCompilationErrorCode(int shaderId)
        {
            return 0;
        }

        public string GetShaderInfoLog(int shaderId)
        {
            return "";
        }

        public string GetStandardFragmentShader()
        {
            return "";
        }

        public string GetStandardVertexShader()
        {
            return "";
        }

        public int GetUniformLocation(int programId, string varName)
        {
            return 0;
        }

        public static GraphicsDevice Device;

        public void Init()
        {
            _graphicsDevice = Device;
            _factory = new DisposeCollectorResourceFactory(_graphicsDevice.ResourceFactory);
            _cl = _factory.CreateCommandList();

            ShaderSetDescription shaderSet = new ShaderSetDescription(
            new[]
            {
                new VertexLayoutDescription(
                    new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
                    new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
            },
            new[]
            {
                loadShader(_factory, "Cube", ShaderStages.Vertex, "VS"),
                loadShader(_factory, "Cube", ShaderStages.Fragment, "FS")
            });

            _projViewLayout = _factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("View", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            _worldTextureLayout = _factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("World", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

            _pipeline = _factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
                DepthStencilStateDescription.DepthOnlyLessEqual,
                RasterizerStateDescription.Default,
                PrimitiveTopology.TriangleList,
                shaderSet,
                new[] { _projViewLayout, _worldTextureLayout },
                _graphicsDevice.MainSwapchain.Framebuffer.OutputDescription));

            _projectionBuffer = _factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            _viewBuffer = _factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            _projViewSet = _factory.CreateResourceSet(new ResourceSetDescription(
                _projViewLayout,
                _projectionBuffer,
                _viewBuffer));
        }

        public void InitPointers(int size)
        {
        }

        public void LineWidth(float lineWidth)
        {
        }

        public void LinkProgram(int programId)
        {
        }

        public void LoadIdentity()
        {
        }

        public void MatrixMode(MatrixType matrix)
        {
        }

        public void Ortho(double left, double right, double bottom, double top, double zNear, double zFar)
        {
        }

        public void SetActiveShader(IShader shader)
        {
        }

        public void SetShaderAppVars()
        {
        }

        public void SetTextureMagFilter(ScaleUpFilters filter)
        {
        }

        public void SetTextureMinFilter(ScaleDownFilters filter)
        {
        }

        public void SetTextureWrapS(TextureWrap wrap)
        {
        }

        public void SetTextureWrapT(TextureWrap wrap)
        {
        }

        public void ShaderSource(int shaderId, string sourceCode)
        {
        }

        public void TexImage2D(uint width, uint height, IntPtr scan0)
        {
            Texture texture = _factory.CreateTexture(new TextureDescription(
                width, height, 1, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled, TextureType.Texture2D));

            Texture staging = _factory.CreateTexture(new TextureDescription(
                width, height, 1, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Staging, TextureType.Texture2D));

            var size = width * height * 4;
            _graphicsDevice.UpdateTexture(staging, scan0, size, 0, 0, 0, width, height, 0, 0, 0);

            _worldBuffer = _factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            CommandList cl = _factory.CreateCommandList();
            var identity = Matrix4x4.Identity;
            cl.Begin();
            cl.CopyTexture(staging, texture);
            cl.UpdateBuffer(_worldBuffer, 0, ref identity);
            cl.End();
            _graphicsDevice.SubmitCommands(cl);

            var textureView = _factory.CreateTextureView(texture);
            
            var worldTextureSet = _factory.CreateResourceSet(new ResourceSetDescription(
                _worldTextureLayout,
                _worldBuffer,
                textureView,
                _graphicsDevice.Aniso4xSampler));

            _textures[_boundTexture] = new TextureContainer { Texture = texture, WorldTextureSet = worldTextureSet };
        }

        public void UndoLastViewport()
        {
        }

        public void Uniform1(int varLocation, int x)
        {
        }

        public void Uniform1(int varLocation, float x)
        {
        }

        public void Uniform2(int varLocation, float x, float y)
        {
        }

        public void Uniform3(int varLocation, float x, float y, float z)
        {
        }

        public void Uniform4(int varLocation, float x, float y, float z, float w)
        {
        }

        public void UseProgram(int programId)
        {
        }

        public void Viewport(int x, int y, int width, int height)
        {
        }

        private Shader loadShader(ResourceFactory factory, string set, ShaderStages stage, string entryPoint)
        {
            string name = $"{set}-{stage.ToString().ToLower()}.{getExtension(factory.BackendType)}";
            return factory.CreateShader(new ShaderDescription(stage, readEmbeddedAssetBytes(name), entryPoint));
        }

        private static string getExtension(GraphicsBackend backendType)
        {
            bool isMacOS = RuntimeInformation.OSDescription.Contains("Darwin");

            return (backendType == GraphicsBackend.Direct3D11)
                ? "hlsl.bytes"
                : (backendType == GraphicsBackend.Vulkan)
                    ? "450.glsl.spv"
                    : (backendType == GraphicsBackend.Metal)
                        ? isMacOS ? "metallib" : "ios.metallib"
                        : (backendType == GraphicsBackend.OpenGL)
                            ? "330.glsl"
                            : "300.glsles";
        }

        private byte[] readEmbeddedAssetBytes(string name)
        {
            using (Stream stream = openEmbeddedAssetStream(name))
            {
                byte[] bytes = new byte[stream.Length];
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    stream.CopyTo(ms);
                    return bytes;
                }
            }
        }

        private Stream openEmbeddedAssetStream(string name) => GetType().Assembly.GetManifestResourceStream(name);

        private BufferUsage getBufferUsage(BufferType type)
        {
            switch (type)
            {
                case BufferType.ArrayBuffer:
                    return BufferUsage.VertexBuffer;
                case BufferType.ElementArrayBuffer:
                    return BufferUsage.IndexBuffer;
                default:
                    throw new NotSupportedException(type.ToString());
            }
        }
    }
}
