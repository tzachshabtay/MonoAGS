namespace AGS.API
{
    /// <summary>
    /// A low level bitmap which can be manipulated.
    /// </summary>
	public interface IBitmap
	{
        /// <summary>
        /// Gets the width of the bitmap (in pixels).
        /// </summary>
        /// <value>The width.</value>
		int Width { get; }

        /// <summary>
        /// Gets the height of the bitmap (in pixels).
        /// </summary>
        /// <value>The height.</value>
		int Height { get; }

        /// <summary>
        /// Clears the bitmap.
        /// </summary>
		void Clear();

        /// <summary>
        /// Gets the pixel color on the specified coordinates.
        /// </summary>
        /// <returns>The pixel color.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
		Color GetPixel(int x, int y);

        /// <summary>
        /// Sets a pixel color on the specified coordinates.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        void SetPixel(Color color, int x, int y);

        /// <summary>
        /// Replaces all pixels with the specified color to be transparent.
        /// </summary>
        /// <param name="color">Color.</param>
		void MakeTransparent(Color color);

        /// <summary>
        /// Uploads the bitmap to graphics memory, and optionally binds to a texture.
        /// </summary>
        /// <param name="textureToBind">Texture to bind.</param>
		void LoadTexture(int? textureToBind);

        /// <summary>
        /// Create a new bitmap with only the pixels which are in the specified area, and makes all of the rest transparent.
        /// </summary>
        /// <returns>The new bitmap.</returns>
        /// <param name="area">Area.</param>
		IBitmap ApplyArea(IArea area);

        /// <summary>
        /// Creates a new bitmap which is cropped to the given rectangle.
        /// </summary>
        /// <returns>The new bitmap.</returns>
        /// <param name="rectangle">Rectangle.</param>
		IBitmap Crop(Rectangle rectangle);

        /// <summary>
        /// Creates a mask from the bitmap.
        /// </summary>
        /// <returns>The mask.</returns>
        /// <param name="factory">The game factory.</param>
        /// <param name="path">The path the bitmap was created from (will be used as the id for the debug mask image, if a debug draw color is given).</param>
        /// <param name="transparentMeansMasked">If set to <c>true</c> then a transparent pixel means masked, otherwise a non transparent pixel means masked.</param>
        /// <param name="debugDrawColor">An optional debug draw color, this will make the mask be saved as an image with the specified color.</param>
        /// <param name="saveMaskToFile">An optional file path to the save the mask (for debugging purposes).</param>
        /// <param name="id">A unique id which will be given to the mask (null will use the path as the id).</param>
		IMask CreateMask(IGameFactory factory, string path, bool transparentMeansMasked = false, 
			Color? debugDrawColor = null, string saveMaskToFile = null, string id = null);

        /// <summary>
        /// Allows to draw text on the bitmap.
        /// </summary>
        /// <returns>The text draw.</returns>
		IBitmapTextDraw GetTextDraw();

        /// <summary>
        /// Saves the bitmap as an image to the specified file path.
        /// </summary>
        /// <param name="path">Path.</param>
        void SaveToFile(string path);
    }
}

