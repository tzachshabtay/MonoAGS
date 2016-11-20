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

		public GLVertex(Vector2 position, Vector2 texCoord, IGLColor color)
			:   this(position, texCoord, color.R, color.G, color.B, color.A)
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

        public static void InitPointers(IGraphicsBackend graphics)
		{
            graphics.InitPointers(Size);
			//graphics.VertexPointer(0, 2, VertexPointerMode.Float, Size, 0);
			//graphics.VertexPointer(1, 2, VertexPointerMode.Float, Size, Vector2.SizeInBytes);
			//graphics.VertexPointer(2, 4, VertexPointerMode.Float, Size, Vector2.SizeInBytes * 2);
		}
	}
}

