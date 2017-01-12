using AGS.API;
using Android.Graphics;
using Android.Text;
using Java.Lang;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace AGS.Engine.Android
{
    public class AndroidFont : IFont
	{
        private static ConcurrentDictionary<string, Typeface> _fontsFromFiles = new ConcurrentDictionary<string, Typeface>();

		public static AndroidFont FromFamilyName(string familyName, FontStyle style, float sizeInPoints)
		{
            AndroidFont font = new AndroidFont ();
            Typeface innerFont = null;
            if (familyName != null) _fontsFromFiles.TryGetValue(familyName, out innerFont);
            if (innerFont != null) innerFont = Typeface.Create(innerFont, style.Convert());
            font.InnerFont = innerFont ?? Typeface.Create(familyName, style.Convert());
			font.FontFamily = familyName;
			font.Style = style;
			font.SizeInPoints = sizeInPoints;
			return font;
		}

		public static AndroidFont FromPath(string path, FontStyle style, float sizeInPoints)
		{
            try
            {
                AndroidFont font = new AndroidFont();
                font.InnerFont = Typeface.Create(Typeface.CreateFromFile(path), style.Convert());
                _fontsFromFiles[path] = font.InnerFont;
                font.FontFamily = path;
                font.Style = style;
                font.SizeInPoints = sizeInPoints;
                return font;
            }
            catch (RuntimeException e)
            {
                Debug.WriteLine(string.Format("Failed to load font from path: {0}, will resort to default font", path));
                Debug.WriteLine(e.ToString());
                return FromFamilyName("sans-serif", style, sizeInPoints);
            }
		}

		public Typeface InnerFont { get; private set; }

		#region IFont implementation

		//http://egoco.de/post/19077604048/calculating-the-height-of-text-in-android
		//http://stackoverflow.com/questions/16082359/how-to-auto-adjust-text-size-on-a-multi-line-textview-according-to-the-view-max
		public SizeF MeasureString(string text, int maxWidth = 2147483647)
		{
            TextPaint paint = AndroidBrush.CreateTextPaint();
            paint.TextSize = SizeInPoints;
            paint.SetTypeface(InnerFont);
            AndroidTextLayout layout = new AndroidTextLayout(paint);
            return layout.MeasureString(text, maxWidth);
		}

        public string FontFamily { get; private set; }

        public FontStyle Style { get; private set; }

        public float SizeInPoints { get; private set; }

        #endregion
        }
    }

