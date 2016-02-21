using System;
using ProtoBuf;
using System.Drawing.Drawing2D;

namespace AGS.Engine
{
	[ProtoContract]
	public class ProtoBlend
	{
		public ProtoBlend()
		{
		}

		[ProtoMember(1)]
		public float[] Factors { get; set; }

		[ProtoMember(2)]
		public float[] Positions { get; set; }

		public static implicit operator Blend(ProtoBlend c)
		{
			Blend blend = new Blend ();
			blend.Factors = c.Factors;
			blend.Positions = c.Positions;
			return blend;
		}

		public static implicit operator ProtoBlend(Blend b)
		{ 
			return new ProtoBlend { Factors = b.Factors, Positions = b.Positions };
		}	
	}
}

