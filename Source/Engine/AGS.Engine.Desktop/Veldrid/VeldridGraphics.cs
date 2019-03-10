using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using AGS.API;
using Veldrid;
using Veldrid.SPIRV;
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
        private readonly Dictionary<(uint, BufferType), DeviceBuffer> _buffers = new Dictionary<(uint, BufferType), DeviceBuffer>();
        private DisposeCollectorResourceFactory _factory;
        private Pipeline _pipeline;
        private ResourceLayout _worldTextureLayout;
        private int _boundTexture;
        private DeviceBuffer _currentVertexBuffer, _currentIndexBuffer, _mvpBuffer;
        private CommandList _cl;
        private Matrix4x4 _ortho, _view;
        private Veldrid.Viewport _viewport, _lastViewport;
        private RgbaFloat _clearColor = RgbaFloat.Black;
        static int _lastTexture = 0;

        public void BeginTick()
        {
            _cl.Begin();
            _cl.SetFramebuffer(_graphicsDevice.MainSwapchain.Framebuffer);
        }

        public void EndTick()
        {
            _cl.End();
            _graphicsDevice.SubmitCommands(_cl);
        }

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
            var totalSize = (uint)(itemSize * items.Length);
            var buffer = _buffers.GetOrAdd((totalSize, bufferType), () =>
                   _factory.CreateBuffer(new BufferDescription(totalSize, getBufferUsage(bufferType))));
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
            _clearColor = new RgbaFloat(r, g, b, a);
        }

        public void ClearScreen()
        {
            _cl.ClearColorTarget(0, _clearColor);
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

            _cl.UpdateBuffer(_mvpBuffer, 0, ref _ortho);

            _cl.SetPipeline(_pipeline);
            _cl.SetVertexBuffer(0, _currentVertexBuffer);
            _cl.SetIndexBuffer(_currentIndexBuffer, IndexFormat.UInt16);
            _cl.SetGraphicsResourceSet(0, worldTextureSet);
            _cl.SetViewport(0, _viewport);
            _cl.DrawIndexed(6, 1, 0, 0, 0);
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
            //_factory.CreateFramebuffer(new FramebufferDescription())
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
            return @"
#version 450
layout(location = 0) in vec2 fsin_texCoords;
layout(location = 1) in vec4 fsin_color;
layout(location = 0) out vec4 fsout_color;
layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler SurfaceSampler;
void main()
{
    fsout_color =  texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords) * fsin_color;
}";
        }

        public string GetStandardVertexShader()
        {
            return @"
#version 450
layout(set = 0, binding = 0) uniform MvpBuffer
{
    mat4 Mvp;
};
layout(location = 0) in vec2 Position;
layout(location = 1) in vec2 TexCoords;
layout(location = 2) in vec4 Color;
layout(location = 0) out vec2 fsin_texCoords;
layout(location = 1) out vec4 fsin_color;
void main()
{
    vec4 pos = vec4(Position, 1., 1.);
    gl_Position = Mvp * pos;
    fsin_texCoords = TexCoords;
    fsin_color = Color;
}";
        }

        public int GetUniformLocation(int programId, string varName)
        {
            return 0;
        }

        public static GraphicsDevice Device;

        public void Init()
        {
            _view = Matrix4x4.CreateLookAt(System.Numerics.Vector3.UnitZ, System.Numerics.Vector3.Zero, System.Numerics.Vector3.UnitY);
            _graphicsDevice = Device;
            _factory = new DisposeCollectorResourceFactory(_graphicsDevice.ResourceFactory);
            _cl = _factory.CreateCommandList();

            ShaderSetDescription shaderSet = new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                        new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                        new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4))
                },
                _factory.CreateFromSpirv(
                    new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(GetStandardVertexShader()), "main"),
                    new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(GetStandardFragmentShader()), "main")));

            _worldTextureLayout = _factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Mvp", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

            _pipeline = _factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
                DepthStencilStateDescription.Disabled,
                new RasterizerStateDescription(FaceCullMode.Back, PolygonFillMode.Solid, FrontFace.CounterClockwise, false, false),
                PrimitiveTopology.TriangleList,
                shaderSet,
                new[] { _worldTextureLayout },
                _graphicsDevice.MainSwapchain.Framebuffer.OutputDescription));

            _mvpBuffer = _factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
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
            var projection = Matrix4x4.CreateOrthographicOffCenter((float)left, (float)right,
                                                                     (float)bottom, (float)top,
                                                                     (float)zNear, (float)zFar);
            _ortho = _view * projection;
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
            if (scan0 != IntPtr.Zero)
            {
                _graphicsDevice.UpdateTexture(staging, scan0, size, 0, 0, 0, width, height, 0, 0, 0);
            }
            var cl = _factory.CreateCommandList();
            cl.Begin();
            cl.CopyTexture(staging, texture);
            cl.End();
            _graphicsDevice.SubmitCommands(cl);
            var textureView = _factory.CreateTextureView(texture);
            
            var worldTextureSet = _factory.CreateResourceSet(new ResourceSetDescription(
                _worldTextureLayout,
                _mvpBuffer,
                textureView,
                _graphicsDevice.Aniso4xSampler));

            _textures[_boundTexture] = new TextureContainer { Texture = texture, WorldTextureSet = worldTextureSet };
        }

        public void UndoLastViewport()
        {
            _viewport = _lastViewport;
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
            _lastViewport = _viewport;
            _viewport = new Veldrid.Viewport(x, y, width, height, -1f, 1f);
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
