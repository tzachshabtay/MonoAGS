﻿using System;

namespace AGS.API
{
    /// <summary>
    /// A bounding box (a square that bounds something)- used for rendering and hit-tests.
    /// </summary>
	public struct AGSBoundingBox
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.AGSBoundingBox"/> struct.
        /// </summary>
        /// <param name="bottomLeft">Bottom left.</param>
        /// <param name="bottomRight">Bottom right.</param>
        /// <param name="topLeft">Top left.</param>
        /// <param name="topRight">Top right.</param>
        public AGSBoundingBox(Vector2 bottomLeft, Vector2 bottomRight, Vector2 topLeft, Vector2 topRight)
            : this(new Vector3(bottomLeft), new Vector3(bottomRight), new Vector3(topLeft), new Vector3(topRight))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.AGSBoundingBox"/> struct.
        /// </summary>
        /// <param name="bottomLeft">Bottom left.</param>
        /// <param name="bottomRight">Bottom right.</param>
        /// <param name="topLeft">Top left.</param>
        /// <param name="topRight">Top right.</param>
		public AGSBoundingBox(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight)
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

            IsValid = !MathUtils.FloatEquals(MinX, MaxX) && !MathUtils.FloatEquals(MinY, MaxY);
		}

        public AGSBoundingBox(float minX, float maxX, float minY, float maxY)
        {
            BottomLeft = new Vector3(minX, minY, 0f);
            BottomRight = new Vector3(maxX, minY, 0f);
            TopLeft = new Vector3(minX, maxY, 0f);
            TopRight = new Vector3(maxX, maxY, 0f);
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            IsValid = !MathUtils.FloatEquals(MinX, MaxX) && !MathUtils.FloatEquals(MinY, MaxY);
		}

		#region AGSBoundingBox implementation

        /// <summary>
        /// Gets the bottom left point.
        /// </summary>
        /// <value>The bottom left point.</value>
		public Vector3 BottomLeft { get; }

        /// <summary>
        /// Gets the top left point.
        /// </summary>
        /// <value>The top left point.</value>
		public Vector3 TopLeft { get; }

        /// <summary>
        /// Gets the bottom right point.
        /// </summary>
        /// <value>The bottom right point.</value>
		public Vector3 BottomRight { get; }

        /// <summary>
        /// Gets the top right point.
        /// </summary>
        /// <value>The top right point.</value>
		public Vector3 TopRight { get; }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
		public float Width => distance(BottomLeft, BottomRight);

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public float Height => distance(BottomLeft, TopLeft);

        /// <summary>
        /// Gets the minimum x of the square.
        /// </summary>
        /// <value>The minimum x.</value>
        public float MinX { get; }

		/// <summary>
		/// Gets the maximum x of the square.
		/// </summary>
		/// <value>The max x.</value>
		public float MaxX { get; }

		/// <summary>
		/// Gets the minimum y of the square.
		/// </summary>
		/// <value>The minimum y.</value>
		public float MinY { get; }

		/// <summary>
		/// Gets the maximum y of the square.
		/// </summary>
		/// <value>The max y.</value>
		public float MaxY { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:AGS.API.AGSBoundingBox"/> is invalid (i.e max is not bigger than min).
        /// </summary>
        /// <value><c>true</c> if is empty; otherwise, <c>false</c>.</value>
        public bool IsValid { get; }

        /// <summary>
        /// Create a cropped bounding box.
        /// </summary>
        /// <returns>The crop info.</returns>
        /// <param name="crop">Crop.</param>
        /// <param name="adjustedScale">Adjusted scale.</param>
        public AGSCropInfo Crop(BoundingBoxType boundingBoxType, ICropSelfComponent crop, PointF adjustedScale)
		{
            if (crop == null) return new AGSCropInfo(this, null, CropFrom.None);
            return crop.Crop(ref this, boundingBoxType, adjustedScale);
		}

		/// <summary>
		/// Is the specified point contained in the square?
		/// </summary>
		/// <returns>True if the point is in the square, false otherwise.</returns>
		/// <param name="point">Point.</param>
		public bool Contains(Vector2 point)
		{
			//http://www.emanueleferonato.com/2012/03/09/algorithm-to-determine-if-a-point-is-inside-a-square-with-mathematics-no-hit-test-involved/       
			if (!IsValid) return false;

			Vector3 a = BottomLeft;
			Vector3 b = BottomRight;
			Vector3 c = TopRight;
			Vector3 d = TopLeft;

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
        public AGSBoundingBox FlipHorizontal() => new AGSBoundingBox(BottomRight, BottomLeft, TopRight, TopLeft);

        public override string ToString() => $"[A={BottomLeft}, B={BottomRight}, C={TopLeft}, D={TopRight}]";

        public override bool Equals(object obj)
		{
            if (obj is AGSBoundingBox) return Equals((AGSBoundingBox)obj);
			return false;
		}

        public override int GetHashCode() => (BottomLeft.GetHashCode() * 397) ^ TopRight.GetHashCode();

        public bool Equals(AGSBoundingBox square)
		{
            return equals(BottomLeft, square.BottomLeft) && equals(BottomRight, square.BottomRight)
                && equals(TopLeft, square.TopLeft) && equals(TopRight, square.TopRight);
		}

        /// <summary>
        /// Is the given box the same size as this box?
        /// </summary>
        /// <returns><c>true</c>, if size is the same, <c>false</c> otherwise.</returns>
        /// <param name="box">Box.</param>
        public bool SameSize(AGSBoundingBox box)
        {
            return MathUtils.FloatEquals(Width, box.Width) && MathUtils.FloatEquals(Height, box.Height);
        }

        /// <summary>
        /// Multiply the box limits with the specified factorX and factorY.
        /// </summary>
        /// <returns>The new multiplied box.</returns>
        /// <param name="factorX">Factor x.</param>
        /// <param name="factorY">Factor y.</param>
        public AGSBoundingBox Multiply(float factorX, float factorY)
        {
            return new AGSBoundingBox(MinX * factorX, MaxX * factorX, MinY * factorY, MaxY * factorY);
        }

		#endregion

        private bool equals(Vector3 a, Vector3 b)
        {
            return MathUtils.FloatEquals(a.X, b.X) && MathUtils.FloatEquals(a.Y, b.Y) && MathUtils.FloatEquals(a.Z, b.Z);
        }

		private float distance(Vector3 a, Vector3 b)
		{
			float dx = a.X - b.X;
			float dy = a.Y - b.Y;
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}

		private float triangleArea(Vector3 a, Vector3 b, Vector2 c)
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
