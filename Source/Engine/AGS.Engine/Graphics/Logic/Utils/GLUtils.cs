﻿using System;
using AGS.API;

namespace AGS.Engine
{
    public class GLUtils : IGLUtils
	{
		private static readonly Vector2 _bottomLeft = new Vector2 (0.0f, 1.0f);
		private static readonly Vector2 _bottomRight = new Vector2 (1.0f, 1.0f);
		private static readonly Vector2 _topRight = new Vector2 (1.0f, 0.0f);
		private static readonly Vector2 _topLeft = new Vector2 (0.0f, 0.0f);

        private static readonly short[] _quadIndices = {3,0,1,  // first triangle (top left - bottom left - bottom right)
                                                       3,1,2}; // second triangle (top left - bottom right - top right)

        private int _vbo, _ebo;

        private readonly IGraphicsBackend _graphics;
        private readonly IRenderMessagePump _messagePump;
        private readonly GLVertex[] _quad, _line;

        public GLUtils(IGraphicsBackend graphics, IRenderMessagePump messagePump)
        {
            _graphics = graphics;
            _messagePump = messagePump;
            _quad = new GLVertex[4];
            _line = new GLVertex[2];
            CurrentGlobalResolution = new SizeF();
        }

        public static Rectangle ScreenViewport { get; private set; }

        public SizeF CurrentResolution => CurrentGlobalResolution;

        public static SizeF CurrentGlobalResolution { get; private set; }

        public void AdjustResolution(int width, int height)
        {
            if ((int)CurrentGlobalResolution.Width == width && (int)CurrentGlobalResolution.Height == height) return;
            CurrentGlobalResolution = new SizeF(width, height);

            _graphics.MatrixMode(MatrixType.Projection);
            _graphics.LoadIdentity();

            _graphics.Ortho(0, width, 0, height, -1, 1);
            
            _graphics.MatrixMode(MatrixType.ModelView);
            _graphics.LoadIdentity();
        }

        public void RefreshViewport(IGameSettings settings, IWindowInfo window, IViewport viewport, bool updateViewportScreenArea)
        {
            var area = viewport.GetScreenArea(settings, window, updateViewportScreenArea);
            ScreenViewport = viewport.ScreenArea;
            _graphics.Viewport(area.X, area.Y, area.Width, area.Height);
        }

		public void GenBuffers()
		{
			_vbo = _graphics.GenBuffer();
            _graphics.BindBuffer(_vbo, BufferType.ArrayBuffer);
            _graphics.InitPointers(GLVertex.Size);
		
            _ebo = _graphics.GenBuffer();
            _graphics.BindBuffer(_ebo, BufferType.ElementArrayBuffer);
		}

        public void DrawQuad(int texture, AGSBoundingBox box, float r, float g, float b, float a)
		{
            _quad[0] = new GLVertex(box.BottomLeft.Xy, _bottomLeft, r, g, b, a);
            _quad[1] = new GLVertex(box.BottomRight.Xy, _bottomRight, r, g, b, a);
            _quad[2] = new GLVertex(box.TopRight.Xy, _topRight, r, g, b, a);
            _quad[3] = new GLVertex(box.TopLeft.Xy, _topLeft, r, g, b, a);
            DrawQuad(texture, _quad);
		}

		public void DrawQuad(int texture, Vector3 bottomLeft, Vector3 bottomRight, 
			Vector3 topLeft, Vector3 topRight, GLColor bottomLeftColor, GLColor bottomRightColor,
			GLColor topLeftColor, GLColor topRightColor)
		{
            _quad[0] = new GLVertex(bottomLeft.Xy, _bottomLeft, bottomLeftColor);
            _quad[1] = new GLVertex(bottomRight.Xy, _bottomRight, bottomRightColor);
            _quad[2] = new GLVertex(topRight.Xy, _topRight, topRightColor);
            _quad[3] = new GLVertex(topLeft.Xy, _topLeft, topLeftColor);
            DrawQuad(texture, _quad);
		}

        public void DrawQuad(int texture, AGSBoundingBox box, GLColor color, FourCorners<Vector2> texturePos)
		{
            _quad[0] = new GLVertex(box.BottomLeft.Xy, texturePos.BottomLeft, color);
            _quad[1] = new GLVertex(box.BottomRight.Xy, texturePos.BottomRight, color);
            _quad[2] = new GLVertex(box.TopRight.Xy, texturePos.TopRight, color);
            _quad[3] = new GLVertex(box.TopLeft.Xy, texturePos.TopLeft, color);

            DrawQuad(texture, _quad);
		}

        public void DrawQuad(int texture, GLVertex[] vertices)
        {
            texture = getTexture(texture);
            _graphics.BindTexture2D(texture);

            _graphics.BufferData(vertices, GLVertex.Size, BufferType.ArrayBuffer);
            _graphics.InitPointers(GLVertex.Size);

            _graphics.BufferData(_quadIndices, sizeof(short), BufferType.ElementArrayBuffer);
            _graphics.SetShaderAppVars();

            _graphics.DrawElements(PrimitiveMode.Triangles, 6, _quadIndices);
        }

        public bool DrawQuad(IFrameBuffer frameBuffer, AGSBoundingBox square, GLVertex[] vertices)
        {
            if (frameBuffer == null) return false;
            vertices[0] = new GLVertex(square.BottomLeft.Xy, _bottomLeft, Colors.White);
            vertices[1] = new GLVertex(square.BottomRight.Xy, _bottomRight, Colors.White);
            vertices[2] = new GLVertex(square.TopRight.Xy, _topRight, Colors.White);
            vertices[3] = new GLVertex(square.TopLeft.Xy, _topLeft, Colors.White);
            DrawQuad(frameBuffer.Texture.ID, vertices);
            return true;
        }

        public IFrameBuffer BeginFrameBuffer(AGSBoundingBox square, IGameSettings settings)
        {
            float width = square.MaxX - square.MinX;
            float height = square.MaxY - square.MinY;
            var aspectRatio = new SizeF(settings.WindowSize.Width / CurrentResolution.Width,
                                        settings.WindowSize.Height / CurrentResolution.Height);

            var frameBuffer = new GLFrameBuffer(new Size((int)Math.Ceiling(width * aspectRatio.Width),
                                                         (int)Math.Ceiling(height * aspectRatio.Height)), _graphics, _messagePump);
            if (!frameBuffer.Begin())
            {
                frameBuffer.End();
                return null;
            }
            return frameBuffer;
        }

        public void DrawTriangleFan(int texture, GLVertex[] vertices)
		{
            texture = getTexture(texture);
            _graphics.BindTexture2D(texture);

            drawArrays(PrimitiveMode.TriangleFan, vertices);
		}

        public void DrawTriangle(int texture, GLVertex[] vertices)
        { 
            texture = getTexture(texture);
            _graphics.BindTexture2D(texture);

            drawArrays(PrimitiveMode.Triangles, vertices);
        }
			
		public void DrawCross(float x, float y, float width, float height,
			float r, float g, float b, float a)
		{
            _quad[0] = new GLVertex(new Vector2(x - width, y - height/10), _bottomLeft, r,g,b,a);
            _quad[1] = new GLVertex(new Vector2(x + width, y - height/10), _bottomRight, r,g,b,a);
            _quad[2] = new GLVertex(new Vector2(x + width, y + height/10), _topRight, r,g,b,a);
            _quad[3] = new GLVertex(new Vector2(x - width, y + height/10), _topLeft, r,g,b,a);
            DrawQuad(0, _quad);

            _quad[0] = new GLVertex(new Vector2(x - width/10, y - height), _bottomLeft, r,g,b,a);
            _quad[1] = new GLVertex(new Vector2(x + width/10, y - height), _bottomRight, r,g,b,a);
            _quad[2] = new GLVertex(new Vector2(x + width/10, y + height), _topRight, r,g,b,a);
            _quad[3] = new GLVertex(new Vector2(x - width/10, y + height), _topLeft, r,g,b,a);
            DrawQuad(0, _quad);
		}

		public void DrawLine(float x1, float y1, float x2, float y2, 
			float width, float r, float g, float b, float a)
		{
            int texture = getTexture(0);
            _graphics.BindTexture2D(texture);
			_graphics.LineWidth (width);
            _line[0] = new GLVertex(new Vector2(x1, y1), _bottomLeft, r, g, b, a);
            _line[1] = new GLVertex(new Vector2(x2, y2), _bottomRight, r, g, b, a);

            drawArrays(PrimitiveMode.Lines, _line);
		}

        private void drawArrays(PrimitiveMode primitive, GLVertex[] vertices)
        {
            _graphics.BufferData(vertices, GLVertex.Size, BufferType.ArrayBuffer);
            _graphics.DrawArrays(primitive, 0, vertices.Length);
        }

        private static int getTexture(int texture)
        {
            return texture == 0 ? GLTextureFactory.EmptyTexture.ID : texture;
        }
	}
}
