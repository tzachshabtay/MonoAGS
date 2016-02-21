using System;
using ProtoBuf;
using System.Drawing;

namespace AGS.Engine
{
	[ProtoContract]
	public class ProtoPointF
	{
		[ProtoMember(1)]
		public float X { get; set; }

		[ProtoMember(2)]
		public float Y { get; set; }

		public static implicit operator PointF(ProtoPointF c)
		{
			return new PointF (c.X, c.Y);
		}

		public static implicit operator ProtoPointF(PointF b)
		{ 
			return new ProtoPointF { X = b.X, Y = b.Y };
		}	
	}
}

