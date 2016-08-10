using System;
using Android.Graphics;
using AGS.API;

namespace AGS.Engine.Android
{
	public static class Extensions
	{
		public static global::Android.Graphics.Color Convert(this AGS.API.Color color)
		{
			return new global::Android.Graphics.Color (color.R, color.G, color.B, color.A);
		}

		public static TypefaceStyle Convert(this FontStyle style)
		{
			if (style.HasFlag(FontStyle.Bold))
			{
				if (!style.HasFlag(FontStyle.Italic)) return TypefaceStyle.Bold;
				return TypefaceStyle.BoldItalic;
			}
			if (style.HasFlag(FontStyle.Italic))
			{
				return TypefaceStyle.Italic;
			}
			//todo: support Underline and Strikeout
			return TypefaceStyle.Normal;
		}

		public static FontStyle Convert(this TypefaceStyle style)
		{
			switch (style)
			{
				case TypefaceStyle.Bold:
					return FontStyle.Bold;
				case TypefaceStyle.BoldItalic:
					return FontStyle.Bold | FontStyle.Italic;
				case TypefaceStyle.Italic:
					return FontStyle.Italic;
				default:
					return FontStyle.Regular;
			}
		}
	}
}

