using System;

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

        /// <summary>
        /// Checkes if the rectangle contains the specified point.
        /// </summary>
        /// <returns>The contains.</returns>
        /// <param name="point">Point.</param>
        public bool Contains(Vector2 point)
        {
            return point.X >= X && point.X <= X + Height && point.Y <= Y && point.Y >= Y - Height;
        }

        public override string ToString()
        {
            return string.Format("[RectangleF: X={0:0.##}, Y={1:0.##}, Width={2:0.##}, Height={3:0.##}]", X, Y, Width, Height);
        }

        [CustomStringValue(CustomStringApplyWhen.CanWrite)]
        public string ToInspectorString()
        {
            return string.Format("{0:0.##},{1:0.##},{2:0.##},{3:0.##}", X, Y, Width, Height);
        }

        public override bool Equals(Object obj)
        {
            RectangleF? other = obj as RectangleF?;
            if (other == null) return false;

            return Equals(other.Value);
        }

        public bool Equals(RectangleF other)
        {
            return MathUtils.FloatEquals(X, other.X) && MathUtils.FloatEquals(Y, other.Y) &&
                   MathUtils.FloatEquals(Width, other.Width) && MathUtils.FloatEquals(Height, other.Height);
        }

        public override int GetHashCode()
        {
            return (((int)X).GetHashCode() * 397) ^ ((int)Y).GetHashCode();
        }
	}
}

