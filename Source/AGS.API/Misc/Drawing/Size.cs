namespace AGS.API
{
    /// <summary>
    /// Represents an integer size (width and height).
    /// </summary>
	public struct Size
	{
		private readonly int _width, _height;

		public Size(int width, int height)
		{
			_width = width;
			_height = height;
		}

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
		public int Width { get { return _width; } }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
		public int Height { get { return _height; } }

		public override string ToString()
		{
			return string.Format("[Size: Width={0}, Height={1}]", Width, Height);
		}
	}
}

