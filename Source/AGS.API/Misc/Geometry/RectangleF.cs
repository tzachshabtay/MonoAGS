namespace AGS.API
{
	/// <summary>
	/// Represents a rectangle in 2D space.
	/// </summary>
	public struct RectangleF
	{
		private readonly float _x, _y, _width, _height;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:AGS.API.Rectangle"/> struct.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public RectangleF(float x, float y, float width, float height)
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
		public float X { get { return _x; } }

		/// <summary>
		/// Gets the y coordinate.
		/// </summary>
		/// <value>The y.</value>
		public float Y { get { return _y; } }

		/// <summary>
		/// Gets the width.
		/// </summary>
		/// <value>The width.</value>
		public float Width { get { return _width; } }

		/// <summary>
		/// Gets the height.
		/// </summary>
		/// <value>The height.</value>
		public float Height { get { return _height; } }
	}
}

