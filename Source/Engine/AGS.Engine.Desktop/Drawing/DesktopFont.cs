using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using AGS.API;
using FontStyle = AGS.API.FontStyle;
using SizeF = AGS.API.SizeF;

namespace AGS.Engine.Desktop
{
    [PropertyFolder]
    [ConcreteImplementation(Browsable = false)]
    [DataContract]
	public class DesktopFont : IFont
	{
        private readonly IFontLoader _fontLoader;

        public DesktopFont(Font font, IFontLoader fontLoader, string path)
		{
            Trace.Assert(font != null, "font is null");
            Trace.Assert(font.FontFamily != null, "font.FontFamily is null");
            InnerFont = font;
            _fontLoader = fontLoader;
            Path = path;
		}

        [Property(Browsable = false)]
		public Font InnerFont { get; }

        #region IFont implementation

        [DataMember]
        public string FontFamily => InnerFont.FontFamily.Name;

        [DataMember]
        public FontStyle Style => (FontStyle)InnerFont.Style;

        [DataMember]
        public float SizeInPoints => InnerFont.SizeInPoints;

        [DataMember]
        public string Path { get; }

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
