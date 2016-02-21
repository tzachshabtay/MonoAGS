using System;
using ProtoBuf;
using System.Drawing;

namespace AGS.Engine
{
	[ProtoContract()]
	public class ProtoFont
	{
		[ProtoMember(1)]
		public string FontFamily { get; set; }

		[ProtoMember(2)]
		public float SizeInPoints { get; set; }

		[ProtoMember(3)]
		public FontStyle Style { get; set; }

		public static implicit operator Font(ProtoFont f) 
		{
			return new Font(f.FontFamily, f.SizeInPoints, f.Style);
		}

		public static implicit operator ProtoFont(Font f) 
		{ 
			return f == null ? null : new ProtoFont 
			{ 
				FontFamily = f.FontFamily.Name, 
				SizeInPoints = f.SizeInPoints, 
				Style = f.Style 
			};
		}
	}
}

