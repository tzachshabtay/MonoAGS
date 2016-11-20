using System;
using System.Diagnostics;
using AGS.API;
//using OpenTK.Graphics.ES11;
using OpenTK.Graphics.ES20;
//using OpenTK.Graphics.OpenGL;

namespace AGS.Engine
{
    public class OpenGLESBackend : IGraphicsBackend
    {
        private OpenTK.Matrix4 _ortho;

        public void Init()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.Texture2D);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            /*GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);*/

            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
        }

        public void ClearColor(float r, float g, float b, float a)
        {
            GL.ClearColor(r, g, b, a);
        }
        public void ClearScreen() { GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); }

        public void BindTexture2D(int textureId) { GL.BindTexture(TextureTarget.Texture2D, textureId); }
        public void TexImage2D(int width, int height, IntPtr scan0)
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba,
                          PixelType.UnsignedByte, scan0);
        }
        public int GenTexture() { return GL.GenTexture(); }
        public void SetTextureMinFilter(ScaleDownFilters filter) { GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, getMinFilter(filter)); }
        public void SetTextureMagFilter(ScaleUpFilters filter) { GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, getMaxFilter(filter)); }
        public void SetTextureWrapS(TextureWrap wrap) { GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, getWrapMode(wrap)); }
        public void SetTextureWrapT(TextureWrap wrap) { GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, getWrapMode(wrap)); }

        public int GenBuffer() { return GL.GenBuffer(); }
        public void BindBuffer(int bufferId) { GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId); }
        public void BufferData(GLVertex[] vertices)
        {
            GL.BufferData<GLVertex>(BufferTarget.ArrayBuffer, (IntPtr)(GLVertex.Size * vertices.Length),
                vertices, BufferUsageHint.StreamDraw);
        }
        public void DrawArrays(PrimitiveMode primitiveType, int first, int count)
        {
            GL.DrawArrays(getPrimitive(primitiveType), first, count);
        }
        public void DrawElements(PrimitiveMode primitiveType, int count, byte[] indices)
        {
            GL.DrawElements<byte>(getPrimitive(primitiveType), count, DrawElementsType.UnsignedByte, indices); 
        }
        //public void VertexPointer(int index, int size, VertexPointerMode vertexType, int stride, int offset) { GL.VertexAttribPointer(index, size, getVertexPointer(vertexType), true, stride, offset); }
        public void InitPointers(int size)
        {
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, true, size, 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, true, size, Vector2.SizeInBytes);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, true, size, Vector2.SizeInBytes * 2);
        }

        public int GenFrameBuffer() { return GL.GenFramebuffer(); }
        public void BindFrameBuffer(int frameBufferId) { GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId); }
        public void FrameBufferTexture2D(int textureId)
        {
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0,
                                    TextureTarget.Texture2D, textureId, 0);
        }
        public bool DrawFrameBuffer()
        {
            /*DrawBuffersEnum[] attachments = new[] { DrawBuffersEnum.ColorAttachment0 };
            GL.DrawBuffers(1, attachments);

            var errorCode = GL.CheckFramebufferStatus(FramebufferTarget.DrawFramebuffer);
            if (errorCode != FramebufferErrorCode.FramebufferComplete || errorCode != FramebufferErrorCode.FramebufferCompleteExt)
            {
                Debug.WriteLine("Cannot create frame buffer. Error: " + errorCode.ToString());
                return false;
            }*/
            return true;
        }
        public void DeleteFrameBuffer(int frameBufferId) { GL.DeleteFramebuffer(frameBufferId); }

        public void Viewport(int x, int y, int width, int height)
        { GL.Viewport(x, y, width, height); }
        public void MatrixMode(MatrixType matrix) { /*GL.MatrixMode(getMatrixMode(matrix));*/ }
        public void LoadIdentity() { /*GL.LoadIdentity();*/ }
        public void Ortho(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            //GL.Ortho(left, right, bottom, top, zNear, zFar);
            _ortho = new OpenTK.Matrix4(
                new OpenTK.Vector4((float)(2 / right), 0f, 0f, -1f),
                new OpenTK.Vector4(0f, (float)(-2 / top), 0f, 1f),
                new OpenTK.Vector4(0f, 0f, 2f / (float)(zFar - zNear), (float)(zNear + zFar) / (float)(zNear - zFar)),
                new OpenTK.Vector4(0f, 0f, 0f, 1f));
        }
        public bool AreShadersSupported() { return true; }
        public void LineWidth(float lineWidth) { GL.LineWidth(lineWidth); }

        public int CreateProgram() { return GL.CreateProgram(); }
        public void UseProgram(int programId) { GL.UseProgram(programId); }
        public void Uniform1(int varLocation, float x) { GL.Uniform1(varLocation, x); }
        public void Uniform2(int varLocation, float x, float y) { GL.Uniform2(varLocation, x, y); }
        public void Uniform3(int varLocation, float x, float y, float z) { GL.Uniform3(varLocation, x, y, z); }
        public void Uniform4(int varLocation, float x, float y, float z, float w) { GL.Uniform4(varLocation, x, y, z, w); }
        //public void UniformMatrix4(int varLocation, ref Matrix4 matrix) { GL.UniformMatrix4(varLocation, false, ref matrix); }
        public void DeleteProgram(int programId) { GL.DeleteProgram(programId); }
        public int GetUniformLocation(int programId, string varName) { return GL.GetUniformLocation(programId, varName); }
        public int CreateShader(ShaderMode shaderType) { return GL.CreateShader(getShaderType(shaderType)); }
        public void ShaderSource(int shaderId, string sourceCode) { GL.ShaderSource(shaderId, sourceCode); }
        public void CompileShader(int shaderId) { GL.CompileShader(shaderId); }
        public string GetShaderInfoLog(int shaderId) { return GL.GetShaderInfoLog(shaderId); }
        public int GetShaderCompilationErrorCode(int shaderId)
        {
            int errorCode;
            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out errorCode);
            return errorCode;
        }
        public void DeleteShader(int shaderId) { GL.DeleteShader(shaderId); }
        public void AttachShader(int programId, int shaderId) { GL.AttachShader(programId, shaderId); }
        public void LinkProgram(int programId) { GL.LinkProgram(programId); }
        public string GetProgramInfoLog(int programId) { return GL.GetProgramInfoLog(programId); }
        public int GetProgramLinkErrorCode(int programId)
        {
            int errorCode;
            GL.GetProgram(programId, GetProgramParameterName.LinkStatus, out errorCode);
            return errorCode;
        }
        public void ActiveTexture(int paramIndex) { GL.ActiveTexture(TextureUnit.Texture0 + paramIndex); }
        public int GetMaxTextureUnits() { return GL.GetInteger(/*GetPName.MaxTextureUnits*/GetPName.MaxTextureImageUnits); }
        public void SetShaderAppVars(int programId)
        {
            var location = GetUniformLocation(programId, "uMvp");
            if (location < 0) return;
            GL.UniformMatrix4(location, false, ref _ortho);
        }

        private int getWrapMode(TextureWrap wrap)
        {
            switch (wrap)
            {
                case TextureWrap.Clamp: return (int)TextureWrapMode.ClampToEdge;
                //case TextureWrap.MirroredRepeat: return (int)TextureWrapMode.MirroredRepeat;
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

        /*private VertexAttribPointerType getVertexPointer(VertexPointerMode mode)
        {
            switch (mode)
            {
                case VertexPointerMode.Byte: return VertexAttribPointerType.Byte;
                case VertexPointerMode.Fixed: return VertexAttribPointerType.Fixed;
                case VertexPointerMode.Float: return VertexAttribPointerType.Float;
                case VertexPointerMode.Short: return VertexAttribPointerType.Short;
                case VertexPointerMode.UnsignedByte: return VertexAttribPointerType.UnsignedByte;
                case VertexPointerMode.UnsignedShort: return VertexAttribPointerType.UnsignedShort;
                default: throw new NotSupportedException(mode.ToString());
            }
        }*/

        private ShaderType getShaderType(ShaderMode shader)
        {
            switch (shader)
            {
                //case ShaderMode.ComputeShader: return ShaderType.ComputeShader;
                case ShaderMode.FragmentShader: return ShaderType.FragmentShader;
                //case ShaderMode.GeometryShader: return ShaderType.GeometryShader;
                //case ShaderMode.GeometryShaderExt: return ShaderType.GeometryShaderExt;
                //case ShaderMode.TessControlShader: return ShaderType.TessControlShader;
                //case ShaderMode.TessEvaluationShader: return ShaderType.TessEvaluationShader;
                case ShaderMode.VertexShader: return ShaderType.VertexShader;
                default: throw new NotSupportedException(shader.ToString());
            }
        }
    }
}
