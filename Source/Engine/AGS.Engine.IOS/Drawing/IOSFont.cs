extern alias IOS;

using AGS.API;
using IOS::CoreGraphics;
using IOS::CoreText;
using IOS::Foundation;

namespace AGS.Engine.IOS
{
    public class IOSFont : IFont
    {
        private readonly FontStyle _style;

        public IOSFont(CTFont font, FontStyle style)
        {
            InnerFont = font;
            _style = style;
        }

        public CTFont InnerFont { get; private set; }

        public string FontFamily { get { return InnerFont.FamilyName; } }

        public float SizeInPoints { get { return (float)InnerFont.Size; } }

        public FontStyle Style { get { return _style; } }

        public SizeF MeasureString(string text, int maxWidth = int.MaxValue)
        {
            NSMutableAttributedString str = new NSMutableAttributedString(text);
            NSRange range = new NSRange(0, text.Length);
            str.SetAttributes(new CTStringAttributes { Font = InnerFont }, range);
            CTFramesetter frame = new CTFramesetter(str);
            NSRange fitRange;
            CGSize size = frame.SuggestFrameSize(range, null, new CGSize(maxWidth, float.MaxValue), out fitRange);
            return new SizeF((float)size.Width, (float)size.Height);
        }
    }
}
