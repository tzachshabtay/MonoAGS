using System;
using System.Diagnostics;
using AGS.API;
using OpenTK.Graphics.OpenGL;

namespace AGS.Engine
{
    public class OpenGLBackend : IGraphicsBackend
    {
        private Rectangle _lastViewport, _currentViewport;
        private string glVersion;

        public void Init()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.Texture2D);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            glVersion = GL.GetString(StringName.Version);
            Debug.WriteLine($"OpenGL Version: {glVersion}");
            Debug.WriteLine($"GLSL Version: {GL.GetString(StringName.ShadingLanguageVersion)}");
        }

        public void ClearColor(float r, float g, float b, float a) => GL.ClearColor(r, g, b, a);
        public void ClearScreen() => GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        public void BindTexture2D(int textureId) => GL.BindTexture(TextureTarget.Texture2D, textureId);
        public void TexImage2D(int width, int height, IntPtr scan0)
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba,
                          PixelType.UnsignedByte, scan0);
        }
        public int GenTexture() => GL.GenTexture();
        public void DeleteTexture(int textureId) => GL.DeleteTexture(textureId);
        public void SetTextureMinFilter(ScaleDownFilters filter) => GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, getMinFilter(filter));
        public void SetTextureMagFilter(ScaleUpFilters filter) => GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, getMaxFilter(filter));
        public void SetTextureWrapS(TextureWrap wrap) => GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, getWrapMode(wrap));
        public void SetTextureWrapT(TextureWrap wrap) => GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, getWrapMode(wrap));

        public int GenBuffer() => GL.GenBuffer();
        public void BindBuffer(int bufferId, BufferType bufferType) 
        {
            var bufferTarget = getBufferTarget(bufferType);
            GL.BindBuffer(bufferTarget, bufferId); 
        }
        public void BufferData<TBufferItem>(TBufferItem[] items, int itemSize, BufferType bufferType) where TBufferItem : struct
        {
            GL.BufferData(getBufferTarget(bufferType), (IntPtr)(itemSize * items.Length), 
                          items, BufferUsageHint.StreamDraw);
        }
        public void DrawElements(PrimitiveMode primitiveType, int count, short[] indices)
        {
            GL.DrawElements(getPrimitive(primitiveType), count, DrawElementsType.UnsignedShort, IntPtr.Zero);
        }
        public void DrawArrays(PrimitiveMode primitiveType, int first, int count)
        {
            GL.DrawArrays(getPrimitive(primitiveType), first, count);
        }
        public void InitPointers(int size)
        {
            GL.VertexPointer(2, VertexPointerType.Float, size, 0);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, size, Vector2.SizeInBytes);
            GL.ColorPointer(4, ColorPointerType.Float, size, Vector2.SizeInBytes * 2);
        }

        public int GenFrameBuffer() => GL.GenFramebuffer();
        public void BindFrameBuffer(int frameBufferId) { GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, frameBufferId); }
        public void FrameBufferTexture2D(int textureId)
        {
            GL.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment0,
                                    TextureTarget.Texture2D, textureId, 0);
        }
        public bool DrawFrameBuffer()
        {
            DrawBuffersEnum[] attachments = new[] { DrawBuffersEnum.ColorAttachment0 };
            GL.DrawBuffers(1, attachments);

            var errorCode = GL.CheckFramebufferStatus(FramebufferTarget.DrawFramebuffer);
            if (errorCode != FramebufferErrorCode.FramebufferComplete || errorCode != FramebufferErrorCode.FramebufferCompleteExt)
            {
                Debug.WriteLine("Cannot create frame buffer. Error: " + errorCode.ToString());
                return false;
            }
            return true;
        }
        public void DeleteFrameBuffer(int frameBufferId) => GL.DeleteFramebuffer(frameBufferId);

        public void Viewport(int x, int y, int width, int height) 
        { 
            _lastViewport = _currentViewport; 
            _currentViewport = new Rectangle(x, y, width, height);
            GL.Viewport(x, y, width, height); 
        }
        public void UndoLastViewport() => Viewport(_lastViewport.X, _lastViewport.Y, _lastViewport.Width, _lastViewport.Height);
        public void MatrixMode(MatrixType matrix) => GL.MatrixMode(getMatrixMode(matrix));
        public void LoadIdentity() => GL.LoadIdentity();
        public void Ortho(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            GL.Ortho(left, right, bottom, top, zNear, zFar);
        }
        public bool AreShadersSupported() 
        {
            return new Version(glVersion.Substring(0, 3)) >= new Version(2, 0);
        }
        public void LineWidth(float lineWidth) => GL.LineWidth(lineWidth);

        public int CreateProgram() => GL.CreateProgram();
        public void UseProgram(int programId) => GL.UseProgram(programId);
        public void Uniform1(int varLocation, int x) => GL.Uniform1(varLocation, x);
        public void Uniform1(int varLocation, float x) => GL.Uniform1(varLocation, x);
        public void Uniform2(int varLocation, float x, float y) => GL.Uniform2(varLocation, x, y);
        public void Uniform3(int varLocation, float x, float y, float z) => GL.Uniform3(varLocation, x, y, z);
        public void Uniform4(int varLocation, float x, float y, float z, float w) => GL.Uniform4(varLocation, x, y, z, w);
        //public void UniformMatrix4(int varLocation, ref Matrix4 matrix) { GL.UniformMatrix4(varLocation, false, ref matrix); }
        public void DeleteProgram(int programId) => GL.DeleteProgram(programId);
        public int GetUniformLocation(int programId, string varName) => GL.GetUniformLocation(programId, varName);
        public int CreateShader(ShaderMode shaderType) => GL.CreateShader(getShaderType(shaderType));
        public void ShaderSource(int shaderId, string sourceCode) => GL.ShaderSource(shaderId, sourceCode);
        public void CompileShader(int shaderId) => GL.CompileShader(shaderId);
        public string GetShaderInfoLog(int shaderId) => GL.GetShaderInfoLog(shaderId);
        public int GetShaderCompilationErrorCode(int shaderId)
        {
            int errorCode;
            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out errorCode);
            return errorCode;
        }
        public void DeleteShader(int shaderId) => GL.DeleteShader(shaderId);
        public void AttachShader(int programId, int shaderId) => GL.AttachShader(programId, shaderId);
        public void LinkProgram(int programId) => GL.LinkProgram(programId);
        public string GetProgramInfoLog(int programId) => GL.GetProgramInfoLog(programId);
        public int GetProgramLinkErrorCode(int programId)
        {
            int errorCode;
            GL.GetProgram(programId, GetProgramParameterName.LinkStatus, out errorCode);
            return errorCode;
        }
        public void ActiveTexture(int paramIndex) => GL.ActiveTexture(TextureUnit.Texture0 + paramIndex);
        public int GetMaxTextureUnits() => GL.GetInteger(GetPName.MaxTextureUnits);
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
                case PrimitiveMode.Quads: return PrimitiveType.Quads;
                case PrimitiveMode.QuadStrip: return PrimitiveType.QuadStrip;
                case PrimitiveMode.Polygon: return PrimitiveType.Polygon;
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
                case MatrixType.Color: return OpenTK.Graphics.OpenGL.MatrixMode.Color;
                case MatrixType.ModelView: return OpenTK.Graphics.OpenGL.MatrixMode.Modelview;
                case MatrixType.ModelView0Ext: return OpenTK.Graphics.OpenGL.MatrixMode.Modelview0Ext;
                case MatrixType.Projection: return OpenTK.Graphics.OpenGL.MatrixMode.Projection;
                case MatrixType.Texture: return OpenTK.Graphics.OpenGL.MatrixMode.Texture;
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
                case ShaderMode.GeometryShaderExt: return ShaderType.GeometryShaderExt;
                case ShaderMode.TessControlShader: return ShaderType.TessControlShader;
                case ShaderMode.TessEvaluationShader: return ShaderType.TessEvaluationShader;
                case ShaderMode.VertexShader: return ShaderType.VertexShader;
                default: throw new NotSupportedException(shader.ToString());
            }
        }

        private BufferTarget getBufferTarget(BufferType bufferType)
        {
            switch (bufferType)
            {
                case BufferType.ArrayBuffer: return BufferTarget.ArrayBuffer;
                case BufferType.ElementArrayBuffer: return BufferTarget.ElementArrayBuffer;
                default: throw new NotSupportedException(bufferType.ToString());
            }
        }
    }
}
