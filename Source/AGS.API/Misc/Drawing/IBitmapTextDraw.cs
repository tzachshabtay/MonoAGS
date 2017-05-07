using System;

namespace AGS.API
{
    /// <summary>
    /// This interface allows drawing text on a bitmap.
    /// </summary>
	public interface IBitmapTextDraw
	{
        /// <summary>
        /// Creates the drawing context which should be disposed after drawing the text.
        /// </summary>
        /// <returns>The context.</returns>
        IDisposable CreateContext();

        /// <summary>
        /// Draws the text on the bitmap.
        /// </summary>
        /// <param name="text">Text to draw.</param>
        /// <param name="config">Configuration to apply when drawing the text.</param>
        /// <param name="textSize">The expected text size.</param>
        /// <param name="baseSize">The size of the text container, based on which the text should be aligned.</param>
        /// <param name="maxWidth">The maximum width allowed for the text (use int.MaxValue for unlimited width).</param>
        /// <param name="height">The expected height of the text after adding padding and outline.</param>
        /// <param name="xOffset">An optional x pixels to offset the drawn text.</param>
		void DrawText(string text, ITextConfig config, AGS.API.SizeF textSize, AGS.API.SizeF baseSize, 
			int maxWidth, int height, float xOffset);
	}
}

