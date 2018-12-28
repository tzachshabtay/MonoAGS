using AGS.API;

namespace AGS.Engine
{
	public struct GLVertex
	{
		//private readonly Vector2 _position, _texCoord;
		//private readonly Vector4 _color;

		public GLVertex(Vector2 position, Vector2 texCoord, Color color)
			:	this(position, texCoord, color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f)
		{
		}

		public GLVertex(Vector2 position, Vector2 texCoord, GLColor color)
			:   this(position, texCoord, color.R, color.G, color.B, color.A)
		{
		}

		public GLVertex(Vector2 position, Vector2 texCoord, float r, float g, float b, float a)
			:	this(position, texCoord, new Vector4(r,g,b,a))
		{
		}

		public GLVertex(Vector2 position, Vector2 texCoord, Vector4 color)
		{
            /*_position = position;
			_texCoord = texCoord;
			_color = color;*/

            PosX = position.X;
            PosY = position.Y;
            PosZ = 0f;
            TexU = texCoord.X;
            TexV = texCoord.Y;
		}

        static GLVertex()
        { 
            unsafe
            {
                Size = (uint)sizeof(GLVertex);
            }
        }

        public static uint Size;

        public float PosX;
        public float PosY;
        public float PosZ;

        public float TexU;
        public float TexV;

        /*public Vector2 Position => _position;
        public Vector2 TexCoord => _texCoord;
        public Vector4 Color => _color;*/
    }
}