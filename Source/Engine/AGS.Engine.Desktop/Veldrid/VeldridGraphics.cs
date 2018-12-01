using System;
using AGS.API;

namespace AGS.Engine.Desktop
{
	public class VeldridGraphics : IGraphicsBackend
    {
        public VeldridGraphics()
        {
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
        }

        public void BufferData<TBufferItem>(TBufferItem[] items, int itemSize, BufferType bufferType) where TBufferItem : struct
        {
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
            return 0;
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

        public void Init()
        {
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

        public void TexImage2D(int width, int height, IntPtr scan0)
        {
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
    }
}
