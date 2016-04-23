using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using AGS.API;

namespace AGS.Engine
{
	public struct GLVertex
	{
		public GLVertex(Vector2 position, Vector2 texCoord, Color color)
			:	this(position, texCoord, color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f)
		{
		}

		public GLVertex(Vector2 position, Vector2 texCoord, float r, float g, float b, float a)
			:	this(position, texCoord, new Vector4(r,g,b,a))
		{
		}

		public GLVertex(Vector2 position, Vector2 texCoord, Vector4 color)
		{
			Position = position;
			TexCoord = texCoord;
			Color = color;
		}

		public static int Size = Vector2.SizeInBytes * 2 + Vector4.SizeInBytes;

		public Vector2 Position { get; private set; }
		public Vector2 TexCoord { get; private set; }
		public Vector4 Color { get; private set; }

		public static void InitPointers()
		{
			GL.VertexPointer(2, VertexPointerType.Float, Size, 0);
			GL.TexCoordPointer(2, TexCoordPointerType.Float, Size, Vector2.SizeInBytes);
			GL.ColorPointer(4, ColorPointerType.Float, Size, Vector2.SizeInBytes * 2);
		}
	}
}

