using System;

namespace AGS.API
{
    /// <summary>
    /// Represents a float point in 2D space.
    /// </summary>
	public struct PointF
	{
        public static PointF Empty = new PointF();

		private readonly float _x, _y;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.PointF"/> struct.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
		public PointF(float x, float y)
		{
			_x = x;
			_y = y;
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

        public static PointF operator +(PointF p1, PointF p2)
        {
            return new PointF(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static PointF operator -(PointF p1, PointF p2)
        {
            return new PointF(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static PointF operator *(PointF p1, PointF p2)
        {
            return new PointF(p1.X * p2.X, p1.Y * p2.Y);
        }

        public static PointF operator /(PointF p1, PointF p2)
        {
            return new PointF(p1.X / p2.X, p1.Y / p2.Y);
        }

        public static PointF operator *(PointF p1, float factor)
        {
            return new PointF(p1.X * factor, p1.Y * factor);
        }

        public static PointF operator /(PointF p1, float factor)
        {
            return new PointF(p1.X / factor, p1.Y / factor);
        }

        public override string ToString ()
		{
			return string.Format ("{0:0.##},{1:0.##}", X, Y);
		}

		public override bool Equals(Object obj) 
		{
            PointF? other = obj as PointF?;
			if (other == null) return false;

            return Equals(other.Value);
		}

        public bool Equals(PointF other)
        { 
            return (X == other.X) && (Y == other.Y);
        }

		public override int GetHashCode() 
		{
            return (X.GetHashCode() * 397) ^ Y.GetHashCode();
		}
	}
}

