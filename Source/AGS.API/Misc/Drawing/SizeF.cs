namespace AGS.API
{
    /// <summary>
    /// Represents a float size (width and height).
    /// </summary>
	public struct SizeF
	{
		private readonly float _width, _height;

		public SizeF(float width, float height)
		{
			_width = width;
			_height = height;
		}

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
		public float Width => _width;

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public float Height => _height;

        public override string ToString() => $"{Width:0.##},{Height:0.##}";

        public override bool Equals(object obj)
        {
            SizeF? other = obj as SizeF?;
            if (other == null) return false;
            return Equals(other.Value);
        }

        public bool Equals(SizeF other) => other.Width == _width && other.Height == _height;

        public override int GetHashCode() => (_width.GetHashCode() * 397) ^ _height.GetHashCode();

        /// <summary>
        /// Scale the size with specified factorX and factorY.
        /// </summary>
        /// <returns>The new size.</returns>
        /// <param name="factorX">The factor in which to scale the width.</param>
        /// <param name="factorY">The factor in which to scale the height.</param>
        public SizeF Scale(float factorX, float factorY) => new SizeF(_width * factorX, _height * factorY);
    }
}

