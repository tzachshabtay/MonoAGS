using System;

namespace AGS.API
{
    /// <summary>
    /// Represents an integer size (width and height).
    /// </summary>
	public struct Size
    {
        public static Size Empty = new Size();

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
		public int Width => _width;

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height => _height;

        public override string ToString() => $"{Width},{Height}";

        public override bool Equals(Object obj) => obj is Size size && Equals(size);

        public bool Equals(Size other) => (Width == other.Width) && (Height == other.Height);

        public override int GetHashCode() => (Width.GetHashCode() * 397) ^ Height.GetHashCode();

        public static implicit operator Size((int width, int height) size) => new Size(size.width, size.height);

        public void Deconstruct(out int width, out int height)
        {
            width = this.Width;
            height = this.Height;
        }
    }
}