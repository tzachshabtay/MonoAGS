using System;

namespace AGS.API
{
	public struct Color
	{
		private const int SHIFT_A = 24;
		private const int SHIFT_R = 16;
		private const int SHIFT_G = 8;
		private const int SHIFT_B = 0;

		private Color(uint argb)
		{
			Value = argb;
		}

		public uint Value { get; private set; }
		public byte R { get { return(byte)((Value >> SHIFT_R)); } }
		public byte G { get { return(byte)((Value >> SHIFT_G)); } }
		public byte B { get { return(byte)((Value >> SHIFT_B)); } }
		public byte A { get { return(byte)((Value >> SHIFT_A)); } }

		public static Color FromArgb(byte a, byte r, byte g, byte b)
		{
			return new Color ((uint)(r << SHIFT_R |
									 g << SHIFT_G |
									 b << SHIFT_B |
									 a << SHIFT_A));
		}

		public static Color FromRgba(byte r, byte g, byte b, byte a)
		{
			return FromArgb(a, r, g, b);
		}

		public static Color FromHexa(uint argb)
		{
			return new Color (argb);
		}
	}
}

