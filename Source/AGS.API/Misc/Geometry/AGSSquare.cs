using System;

namespace AGS.API
{
	/// <summary>
	/// Represents a square.
	/// 
	/// Note: the points are listed as bottom left, bottom right, etc, even though it's not accurate.
	/// A square doesn't really have bottom left, it just has 4 points. 
	/// We use these descriptors for convenience, since each square in our engine was converted from
	/// a rectangle which does have bottom left, etc.
	/// Also, the order of the points is important for the "Contains" calculation.
	/// </summary>
	public struct AGSSquare
	{
		private readonly bool _isEmpty;

		public AGSSquare(PointF bottomLeft, PointF bottomRight, PointF topLeft, PointF topRight)
		{
			BottomLeft = bottomLeft;
			BottomRight = bottomRight;
			TopLeft = topLeft;
			TopRight = topRight;

            //Using min & max specific versions with 4 values and not using params (with MathUtils.Min & Max) as it allocates an array each time
			MinX = min(bottomLeft.X, bottomRight.X, topLeft.X, topRight.X);
			MaxX = max(bottomLeft.X, bottomRight.X, topLeft.X, topRight.X);
			MinY = min(bottomLeft.Y, bottomRight.Y, topLeft.Y, topRight.Y);
			MaxY = max(bottomLeft.Y, bottomRight.Y, topLeft.Y, topRight.Y);

#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
			_isEmpty = MinX == MaxX || MinY == MaxY;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
		}

		/// <summary>
		/// Gets the bottom left point.
		/// </summary>
		/// <value>The bottom left.</value>
		public PointF BottomLeft { get; private set; }

		/// <summary>
		/// Gets the bottom right point.
		/// </summary>
		/// <value>The bottom right.</value>
        public PointF BottomRight { get; private set; }

		/// <summary>
		/// Gets the top left point.
		/// </summary>
		/// <value>The top left.</value>
        public PointF TopLeft { get; private set; }

		/// <summary>
		/// Gets the top right point.
		/// </summary>
		/// <value>The top right.</value>
		public PointF TopRight { get; private set; }

		/// <summary>
		/// Gets the minimum x of the square.
		/// </summary>
		/// <value>The minimum x.</value>
		public float MinX { get; private set; }

		/// <summary>
		/// Gets the maximum x of the square.
		/// </summary>
		/// <value>The max x.</value>
		public float MaxX { get; private set; }

		/// <summary>
		/// Gets the minimum y of the square.
		/// </summary>
		/// <value>The minimum y.</value>
		public float MinY { get; private set; }

		/// <summary>
		/// Gets the maximum y of the square.
		/// </summary>
		/// <value>The max y.</value>
		public float MaxY { get; private set; }

		/// <summary>
		/// Is the specified point contained in the square?
		/// </summary>
		/// <returns>True if the point is in the square, false otherwise.</returns>
		/// <param name="point">Point.</param>
		public bool Contains(PointF point)
		{
			//http://www.emanueleferonato.com/2012/03/09/algorithm-to-determine-if-a-point-is-inside-a-square-with-mathematics-no-hit-test-involved/       
			if (_isEmpty) return false;

			PointF a = BottomLeft;
			PointF b = BottomRight;
			PointF c = TopRight;
			PointF d = TopLeft;

			if (triangleArea(a, b, point) > 0 || triangleArea(b, c, point) > 0 ||
				triangleArea(c, d, point) > 0 || triangleArea(d, a, point) > 0)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Create a new square which is flipped horizontally from the current square.
		/// </summary>
		/// <returns>The new flipped square.</returns>
		public AGSSquare FlipHorizontal()
		{
			return new AGSSquare(BottomRight, BottomLeft, TopRight, TopLeft);
		}

		public override string ToString()
		{
			return string.Format("[A={0}, B={1}, C={2}, D={3}]", BottomLeft, BottomRight, TopLeft, TopRight);
		}

        public override bool Equals(object obj)
        {
            if (obj is AGSSquare) return Equals((AGSSquare)obj);
            return false;

        }

        public override int GetHashCode()
        {
            return (BottomLeft.GetHashCode() * 397) ^ TopRight.GetHashCode();
        }

        public bool Equals(AGSSquare square)
        {
            return BottomLeft.Equals(square.BottomLeft) && BottomRight.Equals(square.BottomRight)
                 && TopLeft.Equals(square.TopLeft) && TopRight.Equals(square.TopRight);
        }

		private float triangleArea(PointF a, PointF b, PointF c)
		{
			return (c.X * b.Y - b.X * c.Y) - (c.X * a.Y - a.X * c.Y) + (b.X * a.Y - a.X * b.Y);
		}

        private static float min(float x1, float x2, float x3, float x4)
        {
            return Math.Min(x1, Math.Min(x2, Math.Min(x3, x4)));
        }

        private static float max(float x1, float x2, float x3, float x4)
        {
            return Math.Max(x1, Math.Max(x2, Math.Max(x3, x4)));
        }
	}
}

