using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace AGS.Engine
{
	public static class GLUtils
	{
		private static Vector2 _bottomLeft = new Vector2 (0.0f, 1.0f);
		private static Vector2 _bottomRight = new Vector2 (1.0f, 1.0f);
		private static Vector2 _topRight = new Vector2 (1.0f, 0.0f);
		private static Vector2 _topLeft = new Vector2 (0.0f, 0.0f);

		private static int vbo;

		public static void GenBuffer()
		{
			vbo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GLVertex.InitPointers();
		}

		public static void DrawQuad(int texture, Vector3 bottomLeft, Vector3 bottomRight, 
			Vector3 topLeft, Vector3 topRight, float r, float g, float b, float a)
		{
			GL.BindTexture (TextureTarget.Texture2D, texture);

			GLVertex[] vertices = new GLVertex[]{ new GLVertex(bottomLeft.Xy, _bottomLeft, r,g,b,a), 
				new GLVertex(bottomRight.Xy, _bottomRight, r,g,b,a), new GLVertex(topRight.Xy, _topRight, r,g,b,a),
				new GLVertex(topLeft.Xy, _topLeft, r,g,b,a)};

			GL.BufferData<GLVertex>(BufferTarget.ArrayBuffer, (IntPtr)(GLVertex.Size * vertices.Length), 
				vertices, BufferUsageHint.StreamDraw);
			GL.DrawArrays(PrimitiveType.Quads, 0, vertices.Length);
		}
			
		public static void DrawCross(float x, float y, float width, float height,
			float r, float g, float b, float a)
		{
			GL.BindTexture (TextureTarget.Texture2D, 0);

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

		public static void DrawQuadBorder(Vector3 bottomLeft, Vector3 bottomRight, 
			Vector3 topLeft, Vector3 topRight, float lineWidth,
			float r, float g, float b, float a)
		{
			GL.BindTexture (TextureTarget.Texture2D, 0);
			GL.PolygonMode (MaterialFace.FrontAndBack, PolygonMode.Line);
			GL.LineWidth(lineWidth);

			GLVertex[] vertices = new GLVertex[]{ new GLVertex(bottomLeft.Xy, _bottomLeft, r,g,b,a), 
				new GLVertex(bottomRight.Xy, _bottomRight, r,g,b,a), new GLVertex(topRight.Xy, _topRight, r,g,b,a),
				new GLVertex(topLeft.Xy, _topLeft, r,g,b,a)};

			GL.BufferData<GLVertex>(BufferTarget.ArrayBuffer, (IntPtr)(GLVertex.Size * vertices.Length), 
				vertices, BufferUsageHint.StreamDraw);
			GL.DrawArrays(PrimitiveType.Quads, 0, vertices.Length);

			GL.PolygonMode (MaterialFace.FrontAndBack, PolygonMode.Fill);
		}
			
		public static void DrawLine(float x1, float y1, float x2, float y2, 
			float width, float r, float g, float b, float a)
		{
			GL.BindTexture (TextureTarget.Texture2D, 0);
			GL.LineWidth (width);
			GLVertex[] vertices = new GLVertex[]{ new GLVertex(new Vector2(x1,y1), _bottomLeft, r,g,b,a), 
				new GLVertex(new Vector2(x2,y2), _bottomRight, r,g,b,a)};
			GL.BufferData<GLVertex>(BufferTarget.ArrayBuffer, (IntPtr)(GLVertex.Size * vertices.Length), 
				vertices, BufferUsageHint.StreamDraw);
			
			GL.DrawArrays(PrimitiveType.Lines, 0, vertices.Length);
		}
	}
}

