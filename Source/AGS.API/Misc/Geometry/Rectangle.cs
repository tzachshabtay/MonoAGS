namespace AGS.API
{
    /// <summary>
    /// Represents a rectangle in 2D space.
    /// </summary>
	public struct Rectangle
	{
		private readonly int _x, _y, _width, _height;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.Rectangle"/> struct.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
		public Rectangle(int x, int y, int width, int height)
		{
			_x = x;
			_y = y;
			_width = width;
			_height = height;
		}

        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        /// <value>The x.</value>
		public int X => _x;

        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        /// <value>The y.</value>
        public int Y => _y;

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width => _width;

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height => _height;

        public override string ToString() => $"[Rectangle: X={X}, Y={Y}, Width={Width}, Height={Height}]";

        [CustomStringValue(CustomStringApplyWhen.CanWrite)]
        public string ToInspectorString() => $"{X},{Y},{Width},{Height}";

        public static implicit operator Rectangle((int x, int y, int width, int height) rect) =>
            new Rectangle(rect.x, rect.y, rect.width, rect.height);

        public void Deconstruct(out int x, out int y, out int width, out int height)
        {
            x = this.X;
            y = this.Y;
            width = this.Width;
            height = this.Height;
        }
    }
}

