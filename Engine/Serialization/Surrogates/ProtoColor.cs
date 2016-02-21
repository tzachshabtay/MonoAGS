using System;
using ProtoBuf;
using System.Drawing;

namespace AGS.Engine
{
	[ProtoContract]
	public struct ProtoColor
	{
		[ProtoMember(1, DataFormat=DataFormat.FixedSize)]
		public uint argb;

		public static implicit operator Color(ProtoColor c) 
			{ return Color.FromArgb((int)c.argb); }

		public static implicit operator ProtoColor(Color c)
			{ return new ProtoColor { argb = (uint)c.ToArgb() }; }
	}
}

