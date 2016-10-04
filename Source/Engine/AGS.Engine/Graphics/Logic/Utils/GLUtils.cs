using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using AGS.API;

namespace AGS.Engine
{
	public static class GLUtils
	{
		private static Vector2 _bottomLeft = new Vector2 (0.0f, 1.0f);
		private static Vector2 _bottomRight = new Vector2 (1.0f, 1.0f);
		private static Vector2 _topRight = new Vector2 (1.0f, 0.0f);
		private static Vector2 _topLeft = new Vector2 (0.0f, 0.0f);

		private static int vbo;

        private static int _lastResolutionWidth, _lastResolutionHeight;

        public static void AdjustResolution(int width, int height)
        {
            if (_lastResolutionWidth == width && _lastResolutionHeight == height) return;
            _lastResolutionWidth = width;
            _lastResolutionHeight = height;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            GL.Ortho(0, width, 0, height, -1, 1);
            
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        public static void RefreshViewport(IGameSettings settings, GameWindow gameWindow)
        { 
            if (settings.PreserveAspectRatio) //http://www.david-amador.com/2013/04/opengl-2d-independent-resolution-rendering/
            {
                float targetAspectRatio = (float)settings.VirtualResolution.Width / settings.VirtualResolution.Height;
                Size screen = new Size(gameWindow.Width, gameWindow.Height);
                int width = screen.Width;
                int height = (int)(width / targetAspectRatio + 0.5f);
                if (height > screen.Height)
                {
                    //It doesn't fit our height, we must switch to pillarbox then
                    height = screen.Height;
                    width = (int)(height * targetAspectRatio + 0.5f);
                }

                // set up the new viewport centered in the backbuffer
                int viewX = (screen.Width / 2) - (width / 2);
                int viewY = (screen.Height / 2) - (height / 2);

                GL.Viewport(viewX, viewY, width, height);
            }
            else
            {
                GL.Viewport(0, 0, gameWindow.Width, gameWindow.Height);
            }
        }

		public static void GenBuffer()
		{
			vbo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GLVertex.InitPointers();
		}

		public static void DrawQuad(int texture, Vector3 bottomLeft, Vector3 bottomRight, 
			Vector3 topLeft, Vector3 topRight, float r, float g, float b, float a)
		{
            texture = getTexture(texture);
			GL.BindTexture (TextureTarget.Texture2D, texture);

			GLVertex[] vertices = new GLVertex[]{ new GLVertex(bottomLeft.Xy, _bottomLeft, r,g,b,a), 
				new GLVertex(bottomRight.Xy, _bottomRight, r,g,b,a), new GLVertex(topRight.Xy, _topRight, r,g,b,a),
				new GLVertex(topLeft.Xy, _topLeft, r,g,b,a)};

			GL.BufferData<GLVertex>(BufferTarget.ArrayBuffer, (IntPtr)(GLVertex.Size * vertices.Length), 
				vertices, BufferUsageHint.StreamDraw);
			GL.DrawArrays(PrimitiveType.Quads, 0, vertices.Length);
		}

		public static void DrawQuad(int texture, Vector3 bottomLeft, Vector3 bottomRight, 
			Vector3 topLeft, Vector3 topRight, IGLColor bottomLeftColor, IGLColor bottomRightColor,
			IGLColor topLeftColor, IGLColor topRightColor)
		{
            texture = getTexture(texture);
			GL.BindTexture (TextureTarget.Texture2D, texture);

			GLVertex[] vertices = new GLVertex[]{ new GLVertex(bottomLeft.Xy, _bottomLeft, bottomLeftColor), 
				new GLVertex(bottomRight.Xy, _bottomRight, bottomRightColor), new GLVertex(topRight.Xy, _topRight, topRightColor),
				new GLVertex(topLeft.Xy, _topLeft, topLeftColor)};

			GL.BufferData<GLVertex>(BufferTarget.ArrayBuffer, (IntPtr)(GLVertex.Size * vertices.Length), 
				vertices, BufferUsageHint.StreamDraw);
			GL.DrawArrays(PrimitiveType.Quads, 0, vertices.Length);
		}

		public static void DrawQuad(int texture, Vector3 bottomLeft, Vector3 bottomRight, 
			Vector3 topLeft, Vector3 topRight, IGLColor color, FourCorners<Vector2> texturePos)
		{
            texture = getTexture(texture);
			GL.BindTexture (TextureTarget.Texture2D, texture);

			GLVertex[] vertices = new GLVertex[]{ new GLVertex(bottomLeft.Xy, texturePos.BottomLeft, color), 
				new GLVertex(bottomRight.Xy, texturePos.BottomRight, color), new GLVertex(topRight.Xy, texturePos.TopRight, color),
				new GLVertex(topLeft.Xy, texturePos.TopLeft, color)};

			GL.BufferData<GLVertex>(BufferTarget.ArrayBuffer, (IntPtr)(GLVertex.Size * vertices.Length), 
				vertices, BufferUsageHint.StreamDraw);
			GL.DrawArrays(PrimitiveType.Quads, 0, vertices.Length);
		}

		public static void DrawTriangleFan(int texture, GLVertex[] vertices)
		{
            texture = getTexture(texture);
			GL.BindTexture (TextureTarget.Texture2D, texture);

			GL.BufferData<GLVertex>(BufferTarget.ArrayBuffer, (IntPtr)(GLVertex.Size * vertices.Length),
				vertices, BufferUsageHint.StreamDraw);
			GL.DrawArrays(PrimitiveType.TriangleFan, 0, vertices.Length);
		}
			
		public static void DrawCross(float x, float y, float width, float height,
			float r, float g, float b, float a)
		{
            int texture = getTexture(0);
			GL.BindTexture (TextureTarget.Texture2D, texture);

			GLVertex[] vertices = new GLVertex[]{ 
				new GLVertex(new Vector2(x - width, y - height/10), _bottomLeft, r,g,b,a), 
				new GLVertex(new Vector2(x + width, y - height/10), _bottomRight, r,g,b,a), 
				new GLVertex(new Vector2(x + width, y + height/10), _topRight, r,g,b,a),
				new GLVertex(new Vector2(x - width, y + height/10), _topLeft, r,g,b,a),

				new GLVertex(new Vector2(x - width/10, y - height), _bottomLeft, r,g,b,a), 
				new GLVertex(new Vector2(x + width/10, y - height), _bottomRight, r,g,b,a), 
				new GLVertex(new Vector2(x + width/10, y + height), _topRight, r,g,b,a),
				new GLVertex(new Vector2(x - width/10, y + height), _topLeft, r,g,b,a)
			};

			GL.BufferData<GLVertex>(BufferTarget.ArrayBuffer, (IntPtr)(GLVertex.Size * vertices.Length), 
				vertices, BufferUsageHint.StreamDraw);
			GL.DrawArrays(PrimitiveType.Quads, 0, vertices.Length);
		}

		public static void DrawLine(float x1, float y1, float x2, float y2, 
			float width, float r, float g, float b, float a)
		{
            int texture = getTexture(0);
			GL.BindTexture (TextureTarget.Texture2D, texture);
			GL.LineWidth (width);
			GLVertex[] vertices = new GLVertex[]{ new GLVertex(new Vector2(x1,y1), _bottomLeft, r,g,b,a), 
				new GLVertex(new Vector2(x2,y2), _bottomRight, r,g,b,a)};
			GL.BufferData<GLVertex>(BufferTarget.ArrayBuffer, (IntPtr)(GLVertex.Size * vertices.Length), 
				vertices, BufferUsageHint.StreamDraw);
			
			GL.DrawArrays(PrimitiveType.Lines, 0, vertices.Length);
		}

        private static int getTexture(int texture)
        {
            return texture == 0 ? GLImageRenderer.EmptyTexture.ID : texture;
        }
	}
}

