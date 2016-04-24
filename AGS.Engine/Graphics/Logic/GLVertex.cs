using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using AGS.API;

namespace AGS.Engine
{
	public struct GLVertex
	{
		private readonly Vector2 _position, _texCoord;
		private readonly Vector4 _color;

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
			_position = position;
			_texCoord = texCoord;
			_color = color;
		}

		public static int Size = Vector2.SizeInBytes * 2 + Vector4.SizeInBytes;

		public Vector2 Position { get { return _position; } }
		public Vector2 TexCoord { get { return _texCoord; } }
		public Vector4 Color { get { return _color; } }

		public static void InitPointers()
		{
			GL.VertexPointer(2, VertexPointerType.Float, Size, 0);
			GL.TexCoordPointer(2, TexCoordPointerType.Float, Size, Vector2.SizeInBytes);
			GL.ColorPointer(4, ColorPointerType.Float, Size, Vector2.SizeInBytes * 2);
		}
	}
}

