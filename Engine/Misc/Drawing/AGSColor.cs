using System;
using AGS.API;
using System.Drawing;

namespace AGS.Engine
{
	public class AGSColor : IColor
	{
		public AGSColor(byte r, byte g, byte b, byte a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public AGSColor(Color c) : this(c.R,c.G,c.B,c.A)
		{
		}

		#region IColor implementation

		public byte R { get; private set; }

		public byte G { get; private set; }

		public byte B { get; private set; }

		public byte A { get; private set; }

		#endregion

		public static implicit operator Color(AGSColor c) 
		{ return Color.FromArgb(c.A,c.R,c.G,c.B); }

		public static implicit operator AGSColor(Color c)
		{ return new AGSColor(c.R,c.G,c.B,c.A); }
	}
}

