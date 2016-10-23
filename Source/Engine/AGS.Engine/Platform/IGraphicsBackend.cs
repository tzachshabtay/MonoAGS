using System;
using AGS.API;
using OpenTK;

namespace AGS.Engine
{
    public enum ShaderMode
    {
        FragmentShader,
        VertexShader,
        GeometryShader,
        GeometryShaderExt,
        TessEvaluationShader,
        TessControlShader,
        ComputeShader,
    }

    public enum MatrixType
    {
        Color,
        ModelView,
        ModelView0Ext,
        Projection,
        Texture,
    }

    public enum PrimitiveMode
    {
        Points,
        Lines,
        LineLoop,
        LineStrip,
        Triangles,
        TriangleStrip,
        TriangleFan,
        Quads,
        QuadStrip,
        Polygon,
        LinesAdjacency,
        LineStripAdjacency,
        TrianglesAdjacency,
        TriangleStripAdjacency,
        Patches,
    }

    public enum VertexPointerMode
    {
        Short,
        Int,
        Float,
        Double,
        HalfFloat,
        UnsignedInt2101010Rev,
        Int2101010Rev,
    }

    public enum TexCoordPointerMode
    {
        Short,
        Int,
        Float,
        Double,
        HalfFloat,
        UnsignedInt2101010Rev,
        Int2101010Rev,
    }

    public enum ColorPointerMode
    {
        Byte,
        UnsignedByte,
        Short,
        UnsignedShort,
        Int,
        UnsignedInt,
        Float,
        Double,
        HalfFloat,
        UnsignedInt2101010Rev,
        Int2101010Rev,
    }

    public interface IGraphicsBackend
    {
        void Init();
        void ClearColor(float r, float g, float b, float a);
        void ClearScreen();

        void BindTexture2D(int textureId);
        void TexImage2D(int width, int height, IntPtr scan0);
        int GenTexture();
        void SetTextureMinFilter(ScaleDownFilters filter);
        void SetTextureMagFilter(ScaleUpFilters filter);
        void SetTextureWrapS(TextureWrap wrap);
        void SetTextureWrapT(TextureWrap wrap);

        int GenBuffer();
        void BindBuffer(int bufferId);
        void BufferData(GLVertex[] vertices);
        void DrawArrays(PrimitiveMode primitiveType, int first, int count);
        void VertexPointer(int size, VertexPointerMode vertexType, int stride, int offset);
        void TexCoordPointer(int size, TexCoordPointerMode texCoordType, int stride, int offset);
        void ColorPointer(int size, ColorPointerMode colorType, int stride, int offset);

        int GenFrameBuffer();
        void BindFrameBuffer(int frameBufferId);
        void FrameBufferTexture2D(int textureId);
        bool DrawFrameBuffer();
        void DeleteFrameBuffer(int frameBufferId);

        void Viewport(int x, int y, int width, int height);
        void MatrixMode(MatrixType matrix);
        void LoadIdentity();
        void Ortho(double left, double right, double bottom, double top, double zNear, double zFar);
        string GetVersion();
        void LineWidth(float lineWidth);

        int CreateProgram();
        void UseProgram(int programId);
        void Uniform1(int varLocation, float x);
        void Uniform2(int varLocation, float x, float y);
        void Uniform3(int varLocation, float x, float y, float z);
        void Uniform4(int varLocation, float x, float y, float z, float w);
        void UniformMatrix4(int varLocation, ref Matrix4 matrix);
        void DeleteProgram(int programId);
        int GetUniformLocation(int programId, string varName);
        int CreateShader(ShaderMode shaderType);
        void ShaderSource(int shaderId, string sourceCode);
        void CompileShader(int shaderId);
        string GetShaderInfoLog(int shaderId);
        int GetShaderCompilationErrorCode(int shaderId);
        void DeleteShader(int shaderId);
        void AttachShader(int programId, int shaderId);
        void LinkProgram(int programId);
        string GetProgramInfoLog(int programId);
        int GetProgramLinkErrorCode(int programId);
        void ActiveTexture(int paramIndex);
        int GetMaxTextureUnits();
    }
}
