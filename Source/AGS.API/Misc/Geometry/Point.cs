namespace AGS.API
{
    /// <summary>
    /// Represents an integer point in 2D space.
    /// </summary>
	public struct Point
	{
        public static Point Empty = new Point();

        private readonly int _x, _y;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.Point"/> struct.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
		public Point(int x, int y)
		{
			_x = x;
			_y = y;
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

        public override string ToString() => $"{X},{Y}";

        public static implicit operator Point((int x, int y) point) => new Point(point.x, point.y);

        public void Deconstruct(out int x, out int y)
        {
            x = this.X;
            y = this.Y;
        }
    }
}