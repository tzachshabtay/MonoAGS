using System;
using AGS.API;
using Android.Graphics;
using Android.Widget;
using Android.Text;

namespace AGS.Engine.Android
{
	public class AndroidFont : IFont
	{
		public AndroidFont(string familyName, FontStyle style, float sizeInPoints)
		{
			InnerFont = Typeface.Create(familyName, style.Convert());
			FontFamily = familyName;
			Style = style;
			SizeInPoints = sizeInPoints;
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
			float height = (float)Math.Floor((double)lineCount * bounds.Height());

			return new SizeF (measuredWidth, height);
		}

		public string FontFamily { get; private set; }

		public FontStyle Style { get; private set; }

		public float SizeInPoints { get; private set; }

		#endregion
	}
}

