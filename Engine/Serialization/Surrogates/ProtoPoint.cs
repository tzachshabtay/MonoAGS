using System;
using ProtoBuf;
using System.Drawing;

namespace AGS.Engine
{
	[ProtoContract]
	public class ProtoPoint
	{
		public ProtoPoint()
		{
		}

		[ProtoMember(1)]
		public int X { get; set; }

		[ProtoMember(2)]
		public int Y { get; set; }

		public static implicit operator Point(ProtoPoint c)
		{
			return new Point(c.X, c.Y);
		}

		public static implicit operator ProtoPoint(Point b)
		{ 
			return new ProtoPoint { X = b.X, Y = b.Y };
		}	
	}
}

