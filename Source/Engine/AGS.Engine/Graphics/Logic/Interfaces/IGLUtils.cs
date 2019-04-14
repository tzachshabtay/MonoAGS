using AGS.API;

namespace AGS.Engine
{
    public interface IGLUtils
    {
        SizeF CurrentResolution { get; }
        GraphicsBackend Backend { get; }
        void AdjustResolution(int width, int height);
        void RefreshViewport(IGameSettings settings, IWindowInfo window, IViewport viewport, bool updateViewportScreenArea);
        void GenBuffers();
        void DrawQuad(int texture, AGSBoundingBox box, float r, float g, float b, float a);
        void DrawQuad(int texture, Vector3 bottomLeft, Vector3 bottomRight,
            Vector3 topLeft, Vector3 topRight, GLColor bottomLeftColor, GLColor bottomRightColor,
                      GLColor topLeftColor, GLColor topRightColor);
        void DrawQuad(int texture, AGSBoundingBox box, GLColor color, FourCorners<Vector2> texturePos);
        void DrawQuad(int texture, GLVertex[] vertices);
        bool DrawQuad(IFrameBuffer frameBuffer, AGSBoundingBox square, GLVertex[] vertices);
        IFrameBuffer BeginFrameBuffer(AGSBoundingBox square, IGameSettings settings);
        void DrawTriangleFan(int texture, GLVertex[] vertices);
        void DrawTriangle(int texture, GLVertex[] vertices);
        void DrawCross(float x, float y, float width, float height,
                       float r, float g, float b, float a);
        void DrawLine(float x1, float y1, float x2, float y2,
                      float width, float r, float g, float b, float a);
    }
}
