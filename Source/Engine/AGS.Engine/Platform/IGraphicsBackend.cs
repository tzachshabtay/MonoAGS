using System;
using AGS.API;

namespace AGS.Engine
{
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
        Byte,
        Fixed,
        Float,
        Short,
        UnsignedByte,
        UnsignedShort,
    }

    public enum BufferType
    {
        ArrayBuffer,
        ElementArrayBuffer,
    }

    public interface IGraphicsBackend
    {
        void Init();
        void ClearColor(float r, float g, float b, float a);
        void ClearScreen();
        void BeginTick();
        void EndTick();

        void BindTexture2D(int textureId);
        void TexImage2D(uint width, uint height, IntPtr scan0);
        int GenTexture();
        void DeleteTexture(int textureId);
        void SetTextureMinFilter(ScaleDownFilters filter);
        void SetTextureMagFilter(ScaleUpFilters filter);
        void SetTextureWrapS(TextureWrap wrap);
        void SetTextureWrapT(TextureWrap wrap);

        int GenBuffer();
        void BindBuffer(int bufferId, BufferType bufferType);
        void BufferData<TBufferItem>(TBufferItem[] items, uint itemSize, BufferType bufferType) where TBufferItem : struct;
        void DrawElements(PrimitiveMode primitiveType, int count, short[] indices);
        void DrawArrays(PrimitiveMode primitiveType, int first, int count);
        void InitPointers(int size);

        int GenFrameBuffer();
        void BindFrameBuffer(int frameBufferId);
        void FrameBufferTexture2D(int textureId);
        bool DrawFrameBuffer();
        void DeleteFrameBuffer(int frameBufferId);

        void Viewport(int x, int y, int width, int height);
        void UndoLastViewport();
        void MatrixMode(MatrixType matrix);
        void LoadIdentity();
        void Ortho(double left, double right, double bottom, double top, double zNear, double zFar);
        void LineWidth(float lineWidth);

        bool AreShadersSupported();
        int CreateProgram(params ShaderVarsBuffer[] shaderVars);
        void UseProgram(int programId);
        void Uniform1(int varLocation, int x);
        void Uniform1(int varLocation, float x);
        void Uniform2(int varLocation, float x, float y);
        void Uniform3(int varLocation, float x, float y, float z);
        void Uniform4(int varLocation, float x, float y, float z, float w);
        //void UniformMatrix4(int varLocation, ref Matrix4 matrix);
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
        void SetShaderAppVars();
        void SetActiveShader(IShader shader);
        string GetStandardVertexShader();
        string GetStandardFragmentShader();

        GraphicsBackend AutoDetect();
        GraphicsBackend Backend { get; }
    }
}
