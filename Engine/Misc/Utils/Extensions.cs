using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;

namespace AGS.Engine
{
	public static class Extensions
	{
		public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey key, Func<TValue> getValue)
		{
			TValue value;
			if (!map.TryGetValue (key, out value)) 
			{
				value = getValue ();
				map.Add (key, value);
			}
			return value;
		}

		public static void Clear(this Bitmap bitmap, Color color)
		{
			//todo: Possibly improve performance by using direct access math
			Graphics g = Graphics.FromImage(bitmap);
			g.Clear(color);
		}

		public static void Clear(this Bitmap bitmap)
		{
			bitmap.Clear(Color.White);
		}

		private static Graphics _graphics = Graphics.FromImage(new Bitmap (1, 1));
		public static SizeF Measure(this string text, Font font, int maxWidth = int.MaxValue)
		{
			_graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			return _graphics.MeasureString(text, font, maxWidth, StringFormat.GenericTypographic);
		}
	}
}

