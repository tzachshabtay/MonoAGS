using System.Collections.Concurrent;
using AGS.API;
using Android.Graphics;
using Android.Text;

namespace AGS.Engine.Android
{
    /// <summary>
    /// Lays out text to measure and render (note: this is similar in functionality to StaticLayout,
    /// though with StaticLayout we saw unexplained spacing).
    /// </summary>
    public class AndroidTextLayout
    {
        private TextPaint _paint;
        private AndroidFontMetrics _metrics;
        private float[] _measuredWidths = { 0 };
        private static ConcurrentDictionary<Typeface, AndroidFontMetrics> _fontsMap = 
            new ConcurrentDictionary<Typeface, AndroidFontMetrics>();

        public AndroidTextLayout(TextPaint textPaint)
        {
            _paint = textPaint;
            _metrics = _fontsMap.GetOrAdd(textPaint.Typeface, _ => getMetrics());
        }

        public SizeF MeasureString(string text, int maxWidth = 2147483647)
        {
            float width = 1f;
            float height = 0f;
            int index = 0;
            int length = text.Length;
            while (index < length - 1)
            {
                string line = getNextLine(text, length, maxWidth, ref index);
                float lineWidth = _paint.MeasureText(line);
                if (lineWidth > width) width = lineWidth;
                height += _metrics.LineHeight;
            }
            return new SizeF(width, height);
        }

        public void DrawString(Canvas gfx, string text, float x, float y, Paint.Align align, int maxWidth = 2147483647)
        {
            y = y + _metrics.LineHeight;
            if (maxWidth != int.MaxValue) _paint.TextAlign = align;

            int index = 0;
            int length = text.Length;
            while (index < length)
            {
                string line = getNextLine(text, length, maxWidth, ref index);
                if (index >= length - 1) y -= _metrics.Descent;
                gfx.DrawText(line, x, y, _paint);
                y += _metrics.LineHeight;
            }
        }

        private string getNextLine(string text, int length, int maxWidth, ref int index)
        { 
            int currentLineLength = _paint.BreakText(text, index, length, true, maxWidth, _measuredWidths);
            int realIndex = index + currentLineLength;
            while (realIndex < length - 1 && realIndex >= index && text[realIndex] != ' ' && text[realIndex + 1] != ' ')
            {
                realIndex--;
            }
            if (realIndex < index) realIndex = index + currentLineLength;
            int start = index;
            index = realIndex;
            if (start != 0 && start != text.Length - 1 && text[start] == ' ') start++;

            return text.Substring(start, index - start);
        }

        private AndroidFontMetrics getMetrics()
        {
            return new AndroidFontMetrics(_paint.GetFontMetrics());
        }

        //todo: On font metrics: http://evendanan.net/2011/12/Font-Metrics-in-Java-OR-How-the-hell-Should-I-Position-This-Font/
        private class AndroidFontMetrics
        {
            public AndroidFontMetrics(Paint.FontMetrics metrics)
            {
                LineHeight = metrics.Descent - metrics.Ascent;
                Descent = metrics.Descent;
            }

            public float LineHeight { get; private set; }
            public float Descent { get; private set; }
        }
    }
}
