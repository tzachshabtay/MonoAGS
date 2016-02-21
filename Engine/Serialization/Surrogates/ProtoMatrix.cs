using System;
using ProtoBuf;
using System.Drawing.Drawing2D;

namespace AGS.Engine
{
	[ProtoContract]
	public class ProtoMatrix
	{
		[ProtoMember(1)]
		public float[] Elements { get; set; }

		public static implicit operator Matrix(ProtoMatrix c)
		{
			return new Matrix (c.Elements[0], c.Elements[1], c.Elements[2],
				c.Elements[3], c.Elements[4], c.Elements[5]);
		}

		public static implicit operator ProtoMatrix(Matrix b)
		{ 
			return new ProtoMatrix { Elements = b.Elements };
		}	
	}
}

