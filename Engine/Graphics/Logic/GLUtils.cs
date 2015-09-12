using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Engine
{
	public static class GLUtils
	{
		public static void DrawQuad(int texture, Vector3 bottomLeft, Vector3 bottomRight, 
			Vector3 topLeft, Vector3 topRight, float r, float g, float b, float a)
		{
			GL.BindTexture (TextureTarget.Texture2D, texture);

			GL.Begin (PrimitiveType.Quads);

			GL.Color4 (r, g, b, a);

			GL.TexCoord2 (0.0f, 1.0f);
			GL.Vertex2(bottomLeft.X, bottomLeft.Y);
			GL.TexCoord2 (1.0f, 1.0f);
			GL.Vertex2(bottomRight.X, bottomRight.Y);
			GL.TexCoord2 (1.0f, 0.0f);
			GL.Vertex2(topRight.X, topRight.Y);
			GL.TexCoord2 (0.0f, 0.0f);
			GL.Vertex2(topLeft.X, topLeft.Y);

			GL.End();
		}

		public static void DrawQuad(int texture, float x, float y, float width, float height, float angle,
			float r, float g, float b, float a)
		{
			bool rotate = angle != 0f;

			GL.BindTexture (TextureTarget.Texture2D, texture);

			if (rotate)
				GL.Rotate (angle, Vector3d.UnitZ);

			GL.Begin (PrimitiveType.Quads);

			GL.Color4 (r, g, b, a);

			GL.TexCoord2 (0.0f, 1.0f);
			GL.Vertex2 (x + 0f, y + 0f);
			GL.TexCoord2 (1.0f, 1.0f);
			GL.Vertex2 (x + width, y + 0f);
			GL.TexCoord2 (1.0f, 0.0f);
			GL.Vertex2 (x + width, y + height);
			GL.TexCoord2 (0.0f, 0.0f);
			GL.Vertex2 (x + 0f, y + height);

			GL.End();

			if (rotate)
				GL.LoadIdentity (); //This cancels the rotation for the next sprite
		}

		public static void DrawCross(float x, float y, float width, float height,
			float r, float g, float b, float a)
		{
			GL.BindTexture (TextureTarget.Texture2D, 0);
			GL.Begin (PrimitiveType.Quads);

			GL.Color4 (r, g, b, a);

			GL.TexCoord2 (0.0f, 1.0f);
			GL.Vertex2 (x - width, y - height/10);
			GL.TexCoord2 (1.0f, 1.0f);
			GL.Vertex2 (x + width, y - height/10);
			GL.TexCoord2 (1.0f, 0.0f);
			GL.Vertex2 (x + width, y + height/10);
			GL.TexCoord2 (0.0f, 0.0f);
			GL.Vertex2 (x - width, y + height/10);

			GL.End();

			GL.Begin (PrimitiveType.Quads);

			GL.Color4 (r, g, b, a);

			GL.TexCoord2 (0.0f, 1.0f);
			GL.Vertex2 (x - width/10, y - height);
			GL.TexCoord2 (1.0f, 1.0f);
			GL.Vertex2 (x + width/10, y - height);
			GL.TexCoord2 (1.0f, 0.0f);
			GL.Vertex2 (x + width/10, y + height);
			GL.TexCoord2 (0.0f, 0.0f);
			GL.Vertex2 (x - width/10, y + height);

			GL.End();
		}

		public static void DrawQuadBorder(Vector3 bottomLeft, Vector3 bottomRight, 
			Vector3 topLeft, Vector3 topRight, float lineWidth,
			float r, float g, float b, float a)
		{
			GL.BindTexture (TextureTarget.Texture2D, 0);
			GL.PolygonMode (MaterialFace.FrontAndBack, PolygonMode.Line);
			GL.LineWidth(lineWidth);
			GL.Begin (PrimitiveType.Quads);

			GL.Color4 (r, g, b, a);

			GL.TexCoord2 (0.0f, 1.0f);
			GL.Vertex2(bottomLeft.X, bottomLeft.Y);
			GL.TexCoord2 (1.0f, 1.0f);
			GL.Vertex2(bottomRight.X, bottomRight.Y);
			GL.TexCoord2 (1.0f, 0.0f);
			GL.Vertex2(topRight.X, topRight.Y);
			GL.TexCoord2 (0.0f, 0.0f);
			GL.Vertex2(topLeft.X, topLeft.Y);

			GL.End();
			GL.PolygonMode (MaterialFace.FrontAndBack, PolygonMode.Fill);

		}

		public static void DrawQuadBorder(float x, float y, float width, float height,
			float r, float g, float b, float a)
		{
			GL.BindTexture (TextureTarget.Texture2D, 0);
			GL.PolygonMode (MaterialFace.FrontAndBack, PolygonMode.Line);
			GL.Begin (PrimitiveType.Quads);

			GL.Color4 (r, g, b, a);

			GL.TexCoord2 (0.0f, 1.0f);
			GL.Vertex2 (x, y);
			GL.TexCoord2 (1.0f, 1.0f);
			GL.Vertex2 (x + width, y);
			GL.TexCoord2 (1.0f, 0.0f);
			GL.Vertex2 (x + width, y + height);
			GL.TexCoord2 (0.0f, 0.0f);
			GL.Vertex2 (x, y + height);

			GL.End();
			GL.PolygonMode (MaterialFace.FrontAndBack, PolygonMode.Fill);

		}

		public static void DrawLine(float x1, float y1, float x2, float y2, 
			float width, float r, float g, float b, float a)
		{
			GL.LineWidth (width);
			GL.Begin (PrimitiveType.Lines);

			GL.Color4 (r, g, b, a);

			GL.Vertex2 (x1, y1);
			GL.Vertex2 (x2, y2);
			GL.End ();
		}
	}
}

