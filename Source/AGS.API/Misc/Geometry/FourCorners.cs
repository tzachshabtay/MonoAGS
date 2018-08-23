using System;

namespace AGS.API
{
    /// <summary>
    /// Represents four values of "something" that matches four corners of a bounding box (for example, four colors for a gradient border).
    /// </summary>
    [ConcreteImplementation(DisplayName = "4-Corners")]
	public class FourCorners<TValue>
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.FourCorners`1"/> class.
        /// </summary>
        /// <param name="value">Value.</param>
		public FourCorners(TValue value) : this(value,value,value,value)
		{
            IsOneValue = true;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.FourCorners`1"/> class.
        /// </summary>
        /// <param name="bottomLeft">Bottom left.</param>
        /// <param name="bottomRight">Bottom right.</param>
        /// <param name="topLeft">Top left.</param>
        /// <param name="topRight">Top right.</param>
        [MethodWizard]
		public FourCorners(TValue bottomLeft, TValue bottomRight, TValue topLeft, TValue topRight)
		{
			BottomLeft = bottomLeft;
			BottomRight = bottomRight;
			TopLeft = topLeft;
			TopRight = topRight;
		}

        /// <summary>
        /// Gets or sets the bottom left value.
        /// </summary>
        /// <value>The bottom left value.</value>
		public TValue BottomLeft { get; set; }

        /// <summary>
        /// Gets or sets the bottom right value.
        /// </summary>
        /// <value>The bottom right value.</value>
		public TValue BottomRight { get; set; }

        /// <summary>
        /// Gets or sets the top left value.
        /// </summary>
        /// <value>The top left value.</value>
		public TValue TopLeft { get; set; }

        /// <summary>
        /// Gets or sets the top right value.
        /// </summary>
        /// <value>The top right value.</value>
		public TValue TopRight { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:AGS.API.FourCorners`1"/> was initialized with a single value.
        /// </summary>
        /// <value><c>true</c> if is one value; otherwise, <c>false</c>.</value>
        public bool IsOneValue { get; }

        /// <summary>
        /// Converts the four corners with a new value.
        /// </summary>
        /// <returns>The convert.</returns>
        /// <param name="convert">Convert.</param>
        /// <typeparam name="TNewValue">The type of the converted object.</typeparam>
		public FourCorners<TNewValue> Convert<TNewValue>(Func<TValue, TNewValue> convert)
		{
            if (IsOneValue) return new FourCorners<TNewValue>(convert(BottomLeft));
			return new FourCorners<TNewValue> (convert(BottomLeft), convert(BottomRight), convert(TopLeft),
				convert(TopRight));
		}

        public bool Equals(FourCorners<TValue> other)
        {
            if (this == other) return true;
            if (other == null) return false;
            return BottomLeft.Equals(other.BottomLeft) && BottomRight.Equals(other.BottomRight)
                             && TopLeft.Equals(other.TopLeft) && TopRight.Equals(other.TopRight);
        }
	}
}

