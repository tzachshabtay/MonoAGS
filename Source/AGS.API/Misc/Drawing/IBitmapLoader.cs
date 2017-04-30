using System.IO;

namespace AGS.API
{
    /// <summary>
    /// This interface allows creating bitmaps.
    /// </summary>
	public interface IBitmapLoader
	{
        /// <summary>
        /// Create a bitmap from a stream of data (this could be loaded from a file, or some other stream like from a network, or from memory).
        /// </summary>
        /// <returns>The bitmap.</returns>
        /// <param name="stream">Stream.</param>
		IBitmap Load(Stream stream);

        /// <summary>
        /// Creates an empty bitmap with the specified width and height (in pixels).
        /// </summary>
        /// <returns>The bitmap.</returns>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
		IBitmap Load(int width, int height);
	}
}

