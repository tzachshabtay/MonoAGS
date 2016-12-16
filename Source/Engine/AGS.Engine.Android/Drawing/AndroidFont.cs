using AGS.API;
using Android.Graphics;
using Android.Text;
using Java.Lang;
using System.Diagnostics;

namespace AGS.Engine.Android
{
	public class AndroidFont : IFont
	{
		public static AndroidFont FromFamilyName(string familyName, FontStyle style, float sizeInPoints)
		{
			AndroidFont font = new AndroidFont ();
			font.InnerFont = Typeface.Create(familyName, style.Convert());
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

			int lineCount = 0;

			int index = 0;
			int length = text.Length;

			float[] measuredWidths = new float[]{ 0 };
			float measuredWidth = 1f;
			while(index < length - 1) 
			{
				index += paint.BreakText(text, index, length, true, maxWidth, measuredWidths);
				lineCount++;
				if (measuredWidth < measuredWidths[0]) measuredWidth = measuredWidths[0];
			}

			Rect bounds = new Rect();
			paint.GetTextBounds("Py", 0, 2, bounds);
			float height = (float)System.Math.Floor((double)lineCount * bounds.Height());

			return new SizeF (measuredWidth, height);
		}

		public string FontFamily { get; private set; }

		public FontStyle Style { get; private set; }

		public float SizeInPoints { get; private set; }

		#endregion
	}
}

