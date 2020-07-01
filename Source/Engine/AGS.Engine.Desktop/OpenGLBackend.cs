using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AGS.API;
using Silk.NET.OpenGL;

namespace AGS.Engine.Desktop
{
    public class OpenGLBackend : IGraphicsBackend
    {
        private Rectangle _lastViewport, _currentViewport;
        private string glVersion;
        private GL _gl;

        public void Init()
        {
            _gl = GL.GetApi(AGSGameWindow.GameWindow.GLContext);
            _gl.Enable(EnableCap.Blend);
            _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _gl.Enable(EnableCap.Texture2D);

            //todo
            //_gl.EnableClientState(ArrayCap.VertexArray);
            //_gl.EnableClientState(ArrayCap.TextureCoordArray);
            //_gl.EnableClientState(ArrayCap.ColorArray);
            //_gl.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest); 

            glVersion = _gl.GetString(StringName.Version);
            Debug.WriteLine($"OpenGL Version: {glVersion}");
            Debug.WriteLine($"GLSL Version: {_gl.GetString(StringName.ShadingLanguageVersion)}");
        }

        public void ClearColor(float r, float g, float b, float a) => _gl.ClearColor(r, g, b, a);
        public void ClearScreen() => _gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

        public void BindTexture2D(int textureId) => _gl.BindTexture(TextureTarget.Texture2D, (uint)textureId);
        public unsafe void TexImage2D(int width, int height, IntPtr scan0)
        {
            _gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, (uint)width, (uint)height, 0, PixelFormat.Bgra,
                          PixelType.UnsignedByte, scan0.ToPointer());
        }
        public int GenTexture() => (int)_gl.GenTexture();
        public void DeleteTexture(int textureId) => _gl.DeleteTexture((uint)textureId);
        public void SetTextureMinFilter(ScaleDownFilters filter) => _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, getMinFilter(filter));
        public void SetTextureMagFilter(ScaleUpFilters filter) => _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, getMaxFilter(filter));
        public void SetTextureWrapS(TextureWrap wrap) => _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, getWrapMode(wrap));
        public void SetTextureWrapT(TextureWrap wrap) => _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, getWrapMode(wrap));

        public int GenBuffer() => (int)_gl.GenBuffer();
        public void BindBuffer(int bufferId, BufferType bufferType) 
        {
            var bufferTarget = getBufferTarget(bufferType);
            _gl.BindBuffer(bufferTarget, (uint)bufferId); 
        }
        public unsafe void BufferData<TBufferItem>(TBufferItem[] items, int itemSize, BufferType bufferType) where TBufferItem : struct
        {
            GCHandle handle = GCHandle.Alloc(items, GCHandleType.Pinned);
            try
            {
                _gl.BufferData(getBufferTarget(bufferType), (UIntPtr)(itemSize * items.Length),
                    handle.AddrOfPinnedObject().ToPointer(), BufferUsageARB.StreamDraw);
            }
            finally
            {
                handle.Free();
            }

        }
        public unsafe void DrawElements(PrimitiveMode primitiveType, int count, short[] indices)
        {
            _gl.DrawElements(getPrimitive(primitiveType), (uint)count, DrawElementsType.UnsignedShort, null);
        }
        public void DrawArrays(PrimitiveMode primitiveType, int first, int count)
        {
            _gl.DrawArrays(getPrimitive(primitiveType), first, (uint)count);
        }
        public void InitPointers(int size)
        {
            /*_gl.VertexPointer(2, VertexPointerType.Float, size, 0);
            _gl.TexCoordPointer(2, TexCoordPointerType.Float, size, Vector2.SizeInBytes);
            _gl.ColorPointer(4, ColorPointerType.Float, size, Vector2.SizeInBytes * 2);*/
        }

        public int GenFrameBuffer() => (int)_gl.GenFramebuffer();
        public void BindFrameBuffer(int frameBufferId) { _gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, (uint)frameBufferId); }
        public void FrameBufferTexture2D(int textureId)
        {
            _gl.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment0,
                                    TextureTarget.Texture2D, (uint)textureId, 0);
        }
        public bool DrawFrameBuffer()
        {
            DrawBufferMode[] attachments = { DrawBufferMode.ColorAttachment0 };
            _gl.DrawBuffers(1, attachments);

            var errorCode = _gl.CheckFramebufferStatus(FramebufferTarget.DrawFramebuffer);
            if (errorCode != (int)ErrorCode.NoError)
            {
                Debug.WriteLine("Cannot create frame buffer. Error: " + errorCode.ToString());
                return false;
            }
            return true;
        }
        public void DeleteFrameBuffer(int frameBufferId) => _gl.DeleteFramebuffer((uint)frameBufferId);

        public void Viewport(int x, int y, int width, int height) 
        { 
            _lastViewport = _currentViewport; 
            _currentViewport = new Rectangle(x, y, width, height);
            _gl.Viewport(x, y, (uint)width, (uint)height); 
        }
        public void UndoLastViewport() => Viewport(_lastViewport.X, _lastViewport.Y, _lastViewport.Width, _lastViewport.Height);
        public void MatrixMode(MatrixType matrix) { } //_gl.MatrixMode(getMatrixMode(matrix));
        public void LoadIdentity() { } //_gl.LoadIdentity();
        public void Ortho(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            //_gl.Ortho(left, right, bottom, top, zNear, zFar);
        }
        public bool AreShadersSupported() 
        {
            return new Version(glVersion.Substring(0, 3)) >= new Version(2, 0);
        }
        public void LineWidth(float lineWidth) => _gl.LineWidth(lineWidth);

        public int CreateProgram() => (int)_gl.CreateProgram();
        public void UseProgram(int programId) => _gl.UseProgram((uint)programId);
        public void Uniform1(int varLocation, int x) => _gl.Uniform1(varLocation, x);
        public void Uniform1(int varLocation, float x) => _gl.Uniform1(varLocation, x);
        public void Uniform2(int varLocation, float x, float y) => _gl.Uniform2(varLocation, x, y);
        public void Uniform3(int varLocation, float x, float y, float z) => _gl.Uniform3(varLocation, x, y, z);
        public void Uniform4(int varLocation, float x, float y, float z, float w) => _gl.Uniform4(varLocation, x, y, z, w);
        //public void UniformMatrix4(int varLocation, ref Matrix4 matrix) { _gl.UniformMatrix4(varLocation, false, ref matrix); }
        public void DeleteProgram(int programId) => _gl.DeleteProgram((uint)programId);
        public int GetUniformLocation(int programId, string varName) => _gl.GetUniformLocation((uint)programId, varName);
        public int CreateShader(ShaderMode shaderType) => (int)_gl.CreateShader(getShaderType(shaderType));
        public void ShaderSource(int shaderId, string sourceCode) => _gl.ShaderSource((uint)shaderId, sourceCode);
        public void CompileShader(int shaderId) => _gl.CompileShader((uint)shaderId);
        public string GetShaderInfoLog(int shaderId) => _gl.GetShaderInfoLog((uint)shaderId);
        public int GetShaderCompilationErrorCode(int shaderId)
        {
            int errorCode;
            _gl.GetShader((uint)shaderId, ShaderParameterName.CompileStatus, out errorCode);
            return errorCode;
        }
        public void DeleteShader(int shaderId) => _gl.DeleteShader((uint)shaderId);
        public void AttachShader(int programId, int shaderId) => _gl.AttachShader((uint)programId, (uint)shaderId);
        public void LinkProgram(int programId) => _gl.LinkProgram((uint)programId);
        public string GetProgramInfoLog(int programId) => _gl.GetProgramInfoLog((uint)programId);
        public int GetProgramLinkErrorCode(int programId)
        {
            int errorCode;
            _gl.GetProgram((uint)programId, ProgramPropertyARB.LinkStatus, out errorCode);
            return errorCode;
        }
        public void ActiveTexture(int paramIndex) => _gl.ActiveTexture(TextureUnit.Texture0 + paramIndex);
        public int GetMaxTextureUnits() => _gl.GetInteger(GetPName.MaxTextureImageUnits);
        public void SetShaderAppVars() { }
        public void SetActiveShader(IShader shader) { }

        public string GetStandardVertexShader()
        {
            return @"#version 120

            varying vec4 vColor;

            void main(void)
            {
                vColor = gl_Color;
                gl_TexCoord[0] = gl_MultiTexCoord0;
                gl_Position = ftransform();
            }
            ";
        }

        public string GetStandardFragmentShader()
        { 
            return @"#version 120

            uniform sampler2D texture;
            varying vec4 vColor;

            void main()
            {
                vec2 pos = gl_TexCoord[0].xy;
                vec4 col = texture2D(texture, pos);
                gl_FragColor = col * vColor;
            }";
        }

        private int getWrapMode(TextureWrap wrap)
        {
            switch (wrap)
            {
                case TextureWrap.Clamp: return (int)TextureWrapMode.ClampToEdge;
                case TextureWrap.MirroredRepeat: return (int)TextureWrapMode.MirroredRepeat;
                case TextureWrap.Repeat: return (int)TextureWrapMode.Repeat;
                default: throw new NotSupportedException(wrap.ToString());
            }
        }

        private int getMinFilter(ScaleDownFilters filters)
        {
            switch (filters)
            {
                case ScaleDownFilters.Nearest: return (int)TextureMinFilter.Nearest;
                case ScaleDownFilters.NearestMipmapLinear: return (int)TextureMinFilter.NearestMipmapLinear;
                case ScaleDownFilters.NearestMipmapNearest: return (int)TextureMinFilter.NearestMipmapNearest;
                case ScaleDownFilters.Linear: return (int)TextureMinFilter.Linear;
                case ScaleDownFilters.LinearMipmapLinear: return (int)TextureMinFilter.LinearMipmapLinear;
                case ScaleDownFilters.LinearMipmapNearest: return (int)TextureMinFilter.LinearMipmapNearest;
                default: throw new NotSupportedException(filters.ToString());
            }
        }

        private int getMaxFilter(ScaleUpFilters filters)
        {
            switch (filters)
            {
                case ScaleUpFilters.Linear: return (int)TextureMagFilter.Linear;
                case ScaleUpFilters.Nearest: return (int)TextureMagFilter.Nearest;
                default: throw new NotSupportedException(filters.ToString());
            }
        }

        private PrimitiveType getPrimitive(PrimitiveMode mode)
        {
            switch (mode)
            {
                case PrimitiveMode.Points: return PrimitiveType.Points;
                case PrimitiveMode.Lines: return PrimitiveType.Lines;
                case PrimitiveMode.LineLoop: return PrimitiveType.LineLoop;
                case PrimitiveMode.LineStrip: return PrimitiveType.LineStrip;
                case PrimitiveMode.Triangles: return PrimitiveType.Triangles;
                case PrimitiveMode.TriangleStrip: return PrimitiveType.TriangleStrip;
                case PrimitiveMode.TriangleFan: return PrimitiveType.TriangleFan;
                case PrimitiveMode.LinesAdjacency: return PrimitiveType.LinesAdjacency;
                case PrimitiveMode.LineStripAdjacency: return PrimitiveType.LineStripAdjacency;
                case PrimitiveMode.TrianglesAdjacency: return PrimitiveType.TrianglesAdjacency;
                case PrimitiveMode.TriangleStripAdjacency: return PrimitiveType.TriangleStripAdjacency;
                case PrimitiveMode.Patches: return PrimitiveType.Patches;
                default: throw new NotSupportedException(mode.ToString());
            }
        }

        private MatrixMode getMatrixMode(MatrixType matrix)
        {
            switch (matrix)
            {
                //case MatrixType.ModelView: return Silk.NET.OpenGL.MatrixMode.Modelview;
                case MatrixType.ModelView0Ext: return Silk.NET.OpenGL.MatrixMode.Modelview0Ext;
                //case MatrixType.Projection: return Silk.NET.OpenGL.MatrixMode.Projection;
                case MatrixType.Texture: return Silk.NET.OpenGL.MatrixMode.Texture;
                default: throw new NotSupportedException(matrix.ToString());
            }
        }

        private ShaderType getShaderType(ShaderMode shader)
        {
            switch (shader)
            {
                case ShaderMode.ComputeShader: return ShaderType.ComputeShader;
                case ShaderMode.FragmentShader: return ShaderType.FragmentShader;
                case ShaderMode.GeometryShader: return ShaderType.GeometryShader;
                case ShaderMode.TessControlShader: return ShaderType.TessControlShader;
                case ShaderMode.TessEvaluationShader: return ShaderType.TessEvaluationShader;
                case ShaderMode.VertexShader: return ShaderType.VertexShader;
                default: throw new NotSupportedException(shader.ToString());
            }
        }

        private BufferTargetARB getBufferTarget(BufferType bufferType)
        {
            switch (bufferType)
            {
                case BufferType.ArrayBuffer: return BufferTargetARB.ArrayBuffer;
                case BufferType.ElementArrayBuffer: return BufferTargetARB.ElementArrayBuffer;
                default: throw new NotSupportedException(bufferType.ToString());
            }
        }
    }
}
