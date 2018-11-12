using System.Drawing;
using AGS.API;
using FontStyle = AGS.API.FontStyle;
using SizeF = AGS.API.SizeF;

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

        public FontStyle Style => (FontStyle)InnerFont.Style;

        public float SizeInPoints => InnerFont.SizeInPoints;

        public SizeF MeasureString(string text, int maxWidth = 2147483647)
		{
			System.Drawing.SizeF size = text.Measure(InnerFont, maxWidth);
			return new SizeF (size.Width, size.Height);
		}

        public IFont Resize(float sizeInPoints)
        {
            return _fontLoader.LoadFont(FontFamily, sizeInPoints, Style);
        }

		public override string ToString()
		{
            return $"{FontFamily}: {SizeInPoints} ({Style})";
		}

		#endregion
	}
}
