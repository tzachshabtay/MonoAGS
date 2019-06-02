using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using AGS.API;
using Veldrid;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;

namespace AGS.Engine.Desktop
{
    public class TextureContainer
    {
        public Texture Texture { get; set; }
        public ResourceSet WorldTextureSet { get; set; }
        public TextureView TextureView { get; set; }
        public ScaleUpFilters UpFilters { get; set; }
        public ScaleDownFilters DownFilters { get; set; }
        public TextureWrap WrapX { get; set; }
        public TextureWrap WrapY { get; set; }
    }

    public class PipelineContainer
    {
        public Framebuffer Framebuffer { get; set; }
        public TextureContainer Output { get; set; }
        public Pipeline Pipeline { get; set; }
    }

    public class ShaderSource
    {
        public string Source { get; set; }
        public ShaderMode Mode { get; set; }
    }

    public class ShaderProgram
    {
        public ShaderProgram()
        {
            Sources = new List<ShaderSource>(); 
        }
        public List<ShaderSource> Sources { get; }
        public ShaderSetDescription Set { get; set; }
        public ResourceLayout VarsLayout { get; set; }
        public ResourceSet VarsSet { get; set; }
        public BindableResource[] GpuVars { get; set; }
        public ShaderVarsBuffer[] VarBuffers { get; set; }
    }

    public class VeldridGraphics : IGraphicsBackend
    {
        private GraphicsDevice _graphicsDevice;
        private readonly Dictionary<int, TextureContainer> _textures = new Dictionary<int, TextureContainer>(1000);
        private readonly Dictionary<(uint, BufferType), DeviceBuffer> _buffers = new Dictionary<(uint, BufferType), DeviceBuffer>();
        private readonly Dictionary<(int,int), PipelineContainer> _pipelines = new Dictionary<(int,int), PipelineContainer>(100);
        private readonly Dictionary<int, Framebuffer> _frameBuffers = new Dictionary<int, Framebuffer>();
        private readonly Dictionary<int, ShaderSource> _shaderSources = new Dictionary<int, ShaderSource>();
        private readonly Dictionary<int, ShaderProgram> _shaders = new Dictionary<int, ShaderProgram>();
        private DisposeCollectorResourceFactory _factory;
        private ResourceLayout _worldTextureLayout;
        private int _boundTexture, _boundFrameBuffer;
        private DeviceBuffer _currentVertexBuffer, _currentIndexBuffer, _mvpBuffer;
        private CommandList _cl;
        private Matrix4x4 _ortho, _view;
        private Veldrid.Viewport _viewport, _lastViewport;
        private RgbaFloat _clearColor = RgbaFloat.Black;
        private int _currentProgram;
        static int _lastTexture = 1, _lastFramebuffer = 1, _lastBuffer = 1,
            _lastShaderSource = 1, _lastShaderProgram = 1;

        public void BeginTick()
        {
            _cl.Begin();
            BindFrameBuffer(_boundFrameBuffer);
            UseProgram(0);
        }

        public void EndTick()
        {
            _cl.End();
            _graphicsDevice.SubmitCommands(_cl);
        }

        public API.GraphicsBackend Backend => _graphicsDevice.BackendType.Convert();

        public void ActiveTexture(int paramIndex)
        {
        }

        public bool AreShadersSupported() => true;

        public void AttachShader(int programId, int shaderId)
        {
            var program = _shaders[programId];
            var source = _shaderSources[shaderId];
            if (!program.Sources.Contains(source))
            {
                program.Sources.Add(source); 
            }
        }

        public void BindBuffer(int bufferId, BufferType bufferType)
        {
        }

        public void BindFrameBuffer(int frameBufferId)
        {
            if (_frameBuffers.TryGetValue(frameBufferId, out var frameBuffer))
            {
                var newContainer = _pipelines.GetOrAdd((frameBufferId, _currentProgram), () => new PipelineContainer { Framebuffer = frameBuffer });
                _cl.SetFramebuffer(newContainer.Framebuffer);
                if (newContainer.Pipeline == null)
                {
                    newContainer.Pipeline = createPipeline(newContainer.Framebuffer); 
                }
                _cl.SetPipeline(newContainer.Pipeline);
            }
            _boundFrameBuffer = frameBufferId;
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
            _cl.UpdateBuffer(buffer, 0, items);
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

        public int CreateProgram(params ShaderVarsBuffer[] vars)
        {
            int id = _lastShaderProgram++;
            var program = new ShaderProgram();
            _shaders[id] = program;
            if (vars.Length > 0)
            {
                var descriptors = new ResourceLayoutElementDescription[vars.Length];
                program.GpuVars = new BindableResource[vars.Length];
                program.VarBuffers = vars;
                for (int i = 0; i < vars.Length; i++)
                {
                    var v = vars[i];
                    descriptors[i] = new ResourceLayoutElementDescription(v.Name, ResourceKind.UniformBuffer, v.Stage.Convert());
                    uint bufLength = (uint)((v.Vars.Length / 16 + 1) * 16); //buffer length must be a multiplication of 16
                    program.GpuVars[i] = _factory.CreateBuffer(new BufferDescription(bufLength, BufferUsage.UniformBuffer));
                }
                program.VarsLayout = _factory.CreateResourceLayout(new ResourceLayoutDescription(descriptors));
                program.VarsSet = _factory.CreateResourceSet(new ResourceSetDescription(program.VarsLayout, program.GpuVars));
            }
            return id;
        }

        public int CreateShader(ShaderMode shaderType)
        {
            int id = _lastShaderSource++;
            _shaderSources[id] = new ShaderSource { Mode = shaderType };
            return id;
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

            _cl.SetVertexBuffer(0, _currentVertexBuffer);
            _cl.SetIndexBuffer(_currentIndexBuffer, IndexFormat.UInt16);
            _cl.SetGraphicsResourceSet(0, worldTextureSet);
            var program = _shaders[_currentProgram];
            if (program.VarsSet != null)
            {
                _cl.SetGraphicsResourceSet(1, program.VarsSet);
            }

            _cl.DrawIndexed(6, 1, 0, 0, 0);
        }

        public bool DrawFrameBuffer()
        {
            return true;
        }

        public int GenBuffer()
        {
            return _lastBuffer++;
        }

        public int GenFrameBuffer()
        {
            var id = _lastFramebuffer++;
            _pipelines[(id, _currentProgram)] = new PipelineContainer();
            return id;
        }

        public void FrameBufferTexture2D(int textureId)
        {
            var texture = _textures[textureId];

            var framebuffer = _factory.CreateFramebuffer(new FramebufferDescription(null, texture.Texture));
            _frameBuffers[_boundFrameBuffer] = framebuffer;
            var container = _pipelines[(_boundFrameBuffer, _currentProgram)];
            container.Framebuffer = framebuffer;
            container.Output = texture;
            container.Pipeline = createPipeline(framebuffer);
            _cl.SetFramebuffer(framebuffer);
            _cl.SetPipeline(container.Pipeline);
        }

        public int GenTexture()
        {
            int texture = _lastTexture++;
            _textures[texture] = new TextureContainer
            {
                WrapX = TextureWrap.Clamp,
                WrapY = TextureWrap.Clamp,
                UpFilters = ScaleUpFilters.Nearest,
                DownFilters = ScaleDownFilters.Nearest,
            };
            return texture;
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
            return 1;
        }

        public int GetShaderCompilationErrorCode(int shaderId)
        {
            return 1;
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
            var shader = _shaders[programId];
            for (int i = 0; i < shader.VarBuffers.Length; i++)
            {
                if (shader.VarBuffers[i].Name == varName)
                    return i; 
            }
            return -1;
        }

        public static GraphicsDevice Device;

        public void Init()
        {
            _view = Matrix4x4.CreateLookAt(System.Numerics.Vector3.UnitZ, System.Numerics.Vector3.Zero, System.Numerics.Vector3.UnitY);
            _graphicsDevice = Device;
            _factory = new DisposeCollectorResourceFactory(_graphicsDevice.ResourceFactory);
            _cl = _factory.CreateCommandList();

            var vertexShader = _lastShaderSource++;
            _shaderSources[vertexShader] = new ShaderSource { Source = GetStandardVertexShader(), Mode = ShaderMode.VertexShader };
            var fragmentShader = _lastShaderSource++;
            _shaderSources[fragmentShader] = new ShaderSource { Source = GetStandardFragmentShader(), Mode = ShaderMode.FragmentShader };

            _shaders[0] = new ShaderProgram();
            AttachShader(0, vertexShader);
            AttachShader(0, fragmentShader);
            LinkProgram(0);

            var framebuffer = _graphicsDevice.MainSwapchain.Framebuffer;
            _frameBuffers[0] = framebuffer;
            var pipeline = new PipelineContainer { Framebuffer = framebuffer };
            _pipelines[(0, 0)] = pipeline;

            _worldTextureLayout = _factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("MvpBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

            _mvpBuffer = _factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            pipeline.Pipeline = createPipeline(framebuffer);
        }

        public API.GraphicsBackend AutoDetect() => VeldridStartup.GetPlatformDefaultBackend().Convert();

        public void InitPointers(int size)
        {
        }

        public void LineWidth(float lineWidth)
        {
        }

        public void LinkProgram(int programId)
        {
            var program = _shaders[programId];
            var vertex = program.Sources.First(s => s.Mode == ShaderMode.VertexShader).Source;
            var fragment = program.Sources.First(s => s.Mode == ShaderMode.FragmentShader).Source;
            var shaderSet = new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                        new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                        new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4))
                },
                _factory.CreateFromSpirv(
                    new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(vertex), "main"),
                    new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(fragment), "main")));
            program.Set = shaderSet;
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
            var tex = _textures[_boundTexture];
            tex.UpFilters = filter;
            refreshTexture(tex);
        }

        public void SetTextureMinFilter(ScaleDownFilters filter)
        {
            var tex = _textures[_boundTexture];
            tex.DownFilters = filter;
            refreshTexture(tex);
        }

        public void SetTextureWrapS(TextureWrap wrap)
        {
            var tex = _textures[_boundTexture];
            tex.WrapX = wrap;
            refreshTexture(tex);
        }

        public void SetTextureWrapT(TextureWrap wrap)
        {
            var tex = _textures[_boundTexture];
            tex.WrapY = wrap;
            refreshTexture(tex);
        }

        public void ShaderSource(int shaderId, string sourceCode)
        {
            _shaderSources[shaderId].Source = sourceCode;
        }

        public void TexImage2D(uint width, uint height, IntPtr scan0)
        {
            Texture texture = _factory.CreateTexture(new TextureDescription(
                width, height, 1, 1, 1, PixelFormat.B8_G8_R8_A8_UNorm, scan0 == IntPtr.Zero ?
                TextureUsage.RenderTarget | TextureUsage.Sampled : TextureUsage.Sampled, TextureType.Texture2D));

            Texture staging = _factory.CreateTexture(new TextureDescription(
                width, height, 1, 1, 1, PixelFormat.B8_G8_R8_A8_UNorm, TextureUsage.Staging, TextureType.Texture2D));

            if (scan0 != IntPtr.Zero)
            {
                var size = width * height * 4;
                _graphicsDevice.UpdateTexture(staging, scan0, size, 0, 0, 0, width, height, 1, 0, 0);
                var cl = _factory.CreateCommandList();
                cl.Begin();
                cl.CopyTexture(staging, texture);
                cl.End();
                _graphicsDevice.SubmitCommands(cl);
            }
            var textureView = _factory.CreateTextureView(texture);

            var container = _textures[_boundTexture];
            container.Texture = texture;
            container.TextureView = textureView;
            refreshTexture(container);
        }

        public void UndoLastViewport()
        {
            _viewport = _lastViewport;
            _cl.SetViewport(0, _viewport);
        }

        public void Uniform1(int varLocation, int x) => Uniform1(varLocation, (float)x);

        public void Uniform1(int varLocation, float x)
        {
            var shader = _shaders[_currentProgram];
            var memBuffer = shader.VarBuffers[varLocation].Vars;
            memBuffer[0] = x;
            var deviceBuffer = (DeviceBuffer)shader.GpuVars[varLocation];
            _cl.UpdateBuffer(deviceBuffer, 0, memBuffer);
        }

        public void Uniform2(int varLocation, float x, float y)
        {
            var shader = _shaders[_currentProgram];
            var memBuffer = shader.VarBuffers[varLocation].Vars;
            memBuffer[0] = x;
            memBuffer[1] = y;
            var deviceBuffer = (DeviceBuffer)shader.GpuVars[varLocation];
            _cl.UpdateBuffer(deviceBuffer, 0, memBuffer);
        }

        public void Uniform3(int varLocation, float x, float y, float z)
        {
            var shader = _shaders[_currentProgram];
            var memBuffer = shader.VarBuffers[varLocation].Vars;
            memBuffer[0] = x;
            memBuffer[1] = y;
            memBuffer[2] = z;
            var deviceBuffer = (DeviceBuffer)shader.GpuVars[varLocation];
            _cl.UpdateBuffer(deviceBuffer, 0, memBuffer);
        }

        public void Uniform4(int varLocation, float x, float y, float z, float w)
        {
            var shader = _shaders[_currentProgram];
            var memBuffer = shader.VarBuffers[varLocation].Vars;
            memBuffer[0] = x;
            memBuffer[1] = y;
            memBuffer[2] = z;
            memBuffer[3] = w;
            var deviceBuffer = (DeviceBuffer)shader.GpuVars[varLocation];
            _cl.UpdateBuffer(deviceBuffer, 0, memBuffer);
        }

        public void UseProgram(int programId)
        {
            _currentProgram = programId;
            var pipeline = _pipelines.GetOrAdd((_boundFrameBuffer, _currentProgram), () => new PipelineContainer { Framebuffer = _frameBuffers[_boundFrameBuffer] });
            if (pipeline.Pipeline == null)
            {
                pipeline.Pipeline = createPipeline(pipeline.Framebuffer); 
            }
            _cl.SetPipeline(pipeline.Pipeline);
        }

        public void Viewport(int x, int y, int width, int height)
        {
            _lastViewport = _viewport;
            var fbHeight = _frameBuffers[_boundFrameBuffer].Height;
            float top = fbHeight - (y + height);
            _viewport = new Viewport(x, top, width, height, -1f, 1f);
            _cl.SetViewport(0, _viewport);
        }

        private Pipeline createPipeline(Framebuffer framebuffer)
        {
            var shader = _shaders[_currentProgram];
            var vars = shader.VarsLayout;
            var desc = new GraphicsPipelineDescription(
                BlendStateDescription.SingleAlphaBlend,
                DepthStencilStateDescription.Disabled,
                new RasterizerStateDescription(FaceCullMode.None, PolygonFillMode.Solid, FrontFace.CounterClockwise, false, false),
                PrimitiveTopology.TriangleList,
                shader.Set,
                vars == null ? new[] { _worldTextureLayout } : new[] { _worldTextureLayout, vars },
                framebuffer.OutputDescription);
            return _factory.CreateGraphicsPipeline(desc);
        }

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

        private SamplerAddressMode getAddress(TextureWrap wrap)
        {
            switch (wrap)
            {
                case TextureWrap.Clamp:
                    return SamplerAddressMode.Clamp;
                case TextureWrap.Repeat:
                    return SamplerAddressMode.Wrap;
                case TextureWrap.MirroredRepeat:
                    return SamplerAddressMode.Mirror;
                default:
                    throw new NotSupportedException(wrap.ToString());
            }
        }

        private SamplerFilter getFilter(ScaleUpFilters upFilters, ScaleDownFilters downFilters)
        {
            switch (downFilters)
            {
                case ScaleDownFilters.Linear:
                case ScaleDownFilters.LinearMipmapLinear:
                    if (upFilters == ScaleUpFilters.Linear) return SamplerFilter.MinLinear_MagLinear_MipLinear;
                    return SamplerFilter.MinLinear_MagPoint_MipLinear;
                case ScaleDownFilters.Nearest:
                case ScaleDownFilters.NearestMipmapNearest:
                    if (upFilters == ScaleUpFilters.Linear) return SamplerFilter.MinPoint_MagLinear_MipPoint;
                    return SamplerFilter.MinPoint_MagPoint_MipPoint;
                case ScaleDownFilters.LinearMipmapNearest:
                    if (upFilters == ScaleUpFilters.Linear) return SamplerFilter.MinLinear_MagLinear_MipPoint;
                    return SamplerFilter.MinLinear_MagPoint_MipPoint;
                case ScaleDownFilters.NearestMipmapLinear:
                    if (upFilters == ScaleUpFilters.Linear) return SamplerFilter.MinPoint_MagLinear_MipLinear;
                    return SamplerFilter.MinPoint_MagPoint_MipLinear;
                default:
                    throw new NotSupportedException(downFilters.ToString());
            }
        }

        private void refreshTexture(TextureContainer container)
        {
            if (container.TextureView == null)
                return;
            var samplerDesc = new SamplerDescription(getAddress(container.WrapX), getAddress(container.WrapY), SamplerAddressMode.Clamp, 
                getFilter(container.UpFilters, container.DownFilters), null, 0, 0, int.MaxValue, 0, SamplerBorderColor.TransparentBlack);
            var sampler = _factory.CreateSampler(samplerDesc);

            var worldTextureSet = _factory.CreateResourceSet(new ResourceSetDescription(
                _worldTextureLayout,
                _mvpBuffer,
                container.TextureView,
                sampler));

            container.WorldTextureSet = worldTextureSet;
        }
    }
}
