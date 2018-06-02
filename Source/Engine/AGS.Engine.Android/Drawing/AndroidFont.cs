using AGS.API;
using Android.Graphics;
using Android.Text;
using Java.Lang;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace AGS.Engine.Android
{
    [PropertyFolder]
    public class AndroidFont : IFont
	{
        private static ConcurrentDictionary<string, Typeface> _fontsFromFiles = new ConcurrentDictionary<string, Typeface>();

        private struct TextMeasureKey
        {
            public TextMeasureKey(string text, Typeface font, float sizeInPoints, int maxWidth)
            {
                Text = text;
                Font = font;
                SizeInPoints = sizeInPoints;
                MaxWidth = maxWidth;
            }
            public string Text;
            public Typeface Font;
            public float SizeInPoints;
            public int MaxWidth;
        }

        private static readonly ConcurrentDictionary<TextMeasureKey, SizeF> _measurements =
            new ConcurrentDictionary<TextMeasureKey, SizeF>();
        
        public static AndroidFont FromFamilyName(string familyName, FontStyle style, float sizeInPoints, IFontLoader fontLoader)
		{
            AndroidFont font = new AndroidFont(fontLoader);
            Typeface innerFont = null;
            if (familyName != null) _fontsFromFiles.TryGetValue(familyName, out innerFont);
            if (innerFont != null) innerFont = Typeface.Create(innerFont, style.Convert());
            font.InnerFont = innerFont ?? Typeface.Create(familyName, style.Convert());
			font.FontFamily = familyName;
			font.Style = style;
			font.SizeInPoints = sizeInPoints;
			return font;
		}

        public static AndroidFont FromPath(string path, FontStyle style, float sizeInPoints, IFontLoader fontLoader)
		{
            try
            {
                AndroidFont font = new AndroidFont(fontLoader);
                font.InnerFont = Typeface.Create(Typeface.CreateFromFile(path), style.Convert());
                _fontsFromFiles[path] = font.InnerFont;
                font.FontFamily = path;
                font.Style = style;
                font.SizeInPoints = sizeInPoints;
                return font;
            }
            catch (RuntimeException e)
            {
                Debug.WriteLine($"Failed to load font from path: {path}, will resort to default font");
                Debug.WriteLine(e.ToString());
                return FromFamilyName("sans-serif", style, sizeInPoints, fontLoader);
            }
		}

        private readonly IFontLoader _fontLoader;

        private AndroidFont(IFontLoader fontLoader)
        {
            _fontLoader = fontLoader;
        }

		public Typeface InnerFont { get; private set; }

		#region IFont implementation

		//http://egoco.de/post/19077604048/calculating-the-height-of-text-in-android
		//http://stackoverflow.com/questions/16082359/how-to-auto-adjust-text-size-on-a-multi-line-textview-according-to-the-view-max
		public SizeF MeasureString(string text, int maxWidth = 2147483647)
		{
            var key = new TextMeasureKey(text, InnerFont, SizeInPoints, maxWidth);
            return _measurements.GetOrAdd(key, k =>
            {
                TextPaint paint = AndroidBrush.CreateTextPaint();
                paint.TextSize = k.SizeInPoints;
                paint.SetTypeface(k.Font);
                AndroidTextLayout layout = new AndroidTextLayout(paint);
                return layout.MeasureString(k.Text, k.MaxWidth);
            });
		}

        public IFont Resize(float sizeInPoints)
        {
            return _fontLoader.LoadFont(FontFamily, sizeInPoints, Style);
        }

        public string FontFamily { get; private set; }

        public FontStyle Style { get; private set; }

        public float SizeInPoints { get; private set; }

        #endregion
        }
    }