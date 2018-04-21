using System;
using AGS.API;
using System.Drawing;

namespace AGS.Engine.Desktop
{
    [PropertyFolder]
	public class DesktopFont : IFont
	{
        private readonly IFontLoader _fontLoader;

        public DesktopFont(Font font, IFontLoader fontLoader)
		{
			InnerFont = font;
            _fontLoader = fontLoader;
		}

		public Font InnerFont { get; }

        #region IFont implementation

        public string FontFamily => InnerFont.FontFamily.Name;

        public AGS.API.FontStyle Style => (AGS.API.FontStyle)InnerFont.Style;

        public float SizeInPoints => InnerFont.SizeInPoints;

        public AGS.API.SizeF MeasureString(string text, int maxWidth = 2147483647)
		{
			System.Drawing.SizeF size = text.Measure(InnerFont, maxWidth);
			return new AGS.API.SizeF (size.Width, size.Height);
		}

        public IFont Resize(float sizeInPoints)
        {
            return _fontLoader.LoadFont(FontFamily, sizeInPoints, Style);
        }

		#endregion
	}
}