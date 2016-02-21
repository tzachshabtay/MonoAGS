using System;
using ProtoBuf;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace AGS.Engine
{
	[ProtoContract]
	public class ProtoColorBlend
	{
		[ProtoMember(1)]
		public Color[] Colors { get; set; }

		[ProtoMember(2)]
		public float[] Positions { get; set; }

		public static implicit operator ColorBlend(ProtoColorBlend c)
		{
			ColorBlend blend = new ColorBlend ();
			blend.Colors = c.Colors;
			blend.Positions = c.Positions;
			return blend;
		}

		public static implicit operator ProtoColorBlend(ColorBlend b)
		{ 
			return new ProtoColorBlend { Colors = b.Colors, Positions = b.Positions };
		}	
	}
}

