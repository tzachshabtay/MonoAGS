using AGS.API;

namespace AGS.Engine
{
	public struct GLVertex
	{
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
            PosX = position.X;
            PosY = position.Y;
            TexU = texCoord.X;
            TexV = texCoord.Y;
            ColR = color.X;
            ColG = color.Y;
            ColB = color.Z;
            ColA = color.W;
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

        public float TexU;
        public float TexV;

        public float ColR;
        public float ColG;
        public float ColB;
        public float ColA;
    }
}