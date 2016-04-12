using System;
using AGS.API;
using System.Drawing;

namespace AGS.Engine
{
	public class AGSFont : IFont
	{
		public AGSFont(Font font)
		{
			InnerFont = font;
		}

		public Font InnerFont { get; private set; }

		#region IFont implementation

		public AGS.API.SizeF MeasureString(string text, int maxWidth = 2147483647)
		{
			System.Drawing.SizeF size = text.Measure(InnerFont, maxWidth);
			return new AGS.API.SizeF (size.Width, size.Height);
		}

		#endregion
	}
}

