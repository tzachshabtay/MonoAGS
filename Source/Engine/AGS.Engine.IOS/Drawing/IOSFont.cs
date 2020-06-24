extern alias IOS;

using System.Collections.Concurrent;
using AGS.API;
using IOS::CoreGraphics;
using IOS::CoreText;
using IOS::Foundation;

namespace AGS.Engine.IOS
{
    [PropertyFolder]
    public class IOSFont : IFont
    {
        private readonly FontStyle _style;
        private readonly IFontLoader _fontLoader;

        private struct TextMeasureKey
        {
            public TextMeasureKey(string text, CTFont font, int maxWidth)
            {
                Text = text;
                Font = font;
                MaxWidth = maxWidth;
            }
            public string Text;
            public CTFont Font;
            public int MaxWidth;
        }

        private static readonly ConcurrentDictionary<TextMeasureKey, SizeF> _measurements =
            new ConcurrentDictionary<TextMeasureKey, SizeF>();
        
        public IOSFont(CTFont font, FontStyle style, IFontLoader fontLoader)
        {
            InnerFont = font;
            _style = style;
            _fontLoader = fontLoader;
        }

        public CTFont InnerFont { get; }

        public string FontFamily => InnerFont.FamilyName;

        public float SizeInPoints => (float)InnerFont.Size;

        public FontStyle Style => _style;

        public SizeF MeasureString(string text, Alignment alignment, int maxWidth = int.MaxValue)
        {
            //todo: support alignment
            var key = new TextMeasureKey(text, InnerFont, maxWidth);
            return _measurements.GetOrAdd(key, k =>
            {
                using (NSMutableAttributedString str = new NSMutableAttributedString(text))
                {
                    NSRange range = new NSRange(0, k.Text.Length);

                    str.SetAttributes(new CTStringAttributes
                    {
                        Font = k.Font,
                    }, range);

                    using (CTFramesetter frameSetter = new CTFramesetter(str))
                    {
                        NSRange fitRange;
                        CGSize size = frameSetter.SuggestFrameSize(range, null, new CGSize(k.MaxWidth, float.MaxValue), out fitRange);
                        return new SizeF((float)size.Width, (float)size.Height);
                    }
                }
            });
        }

        public IFont Resize(float sizeInPoints)
        {
            return _fontLoader.LoadFont(FontFamily, sizeInPoints, Style);
        }
    }
}