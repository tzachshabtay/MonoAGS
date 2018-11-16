using System;
using System.Runtime.Serialization;

namespace AGS.API
{
    /// <summary>
    /// Represents four values of "something" that matches four corners of a bounding box (for example, four colors for a gradient border).
    /// </summary>
    [ConcreteImplementation(DisplayName = "4-Corners")]
    [DataContract]
    public class FourCorners<TValue> : IEquatable<FourCorners<TValue>>
	{
        private TValue _bottomLeft, _bottomRight, _topLeft, _topRight;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.FourCorners`1"/> class.
        /// </summary>
        public FourCorners() : this(default){}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.FourCorners`1"/> class.
        /// </summary>
        /// <param name="value">Value.</param>
		public FourCorners(TValue value) : this(value,value,value,value) {}

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
            _bottomLeft = bottomLeft;
			_bottomRight = bottomRight;
			_topLeft = topLeft;
			_topRight = topRight;
            refreshOneValue();
		}

        /// <summary>
        /// Gets or sets the bottom left value.
        /// </summary>
        /// <value>The bottom left value.</value>
        [DataMember]
		public TValue BottomLeft
        {
            get => _bottomLeft;
            set { _bottomLeft = value; refreshOneValue(); }
        }

        /// <summary>
        /// Gets or sets the bottom right value.
        /// </summary>
        /// <value>The bottom right value.</value>
        [DataMember]
		public TValue BottomRight
        {
            get => _bottomRight;
            set { _bottomRight = value; refreshOneValue(); }
        }

        /// <summary>
        /// Gets or sets the top left value.
        /// </summary>
        /// <value>The top left value.</value>
        [DataMember]
        public TValue TopLeft
        {
            get => _topLeft;
            set { _topLeft = value; refreshOneValue(); }
        }

        /// <summary>
        /// Gets or sets the top right value.
        /// </summary>
        /// <value>The top right value.</value>
        [DataMember]
        public TValue TopRight
        {
            get => _topRight;
            set { _topRight = value; refreshOneValue(); }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:AGS.API.FourCorners`1"/> contains only a single value.
        /// </summary>
        /// <value><c>true</c> if is one value; otherwise, <c>false</c>.</value>
        public bool IsOneValue { get; private set; }

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

        public override bool Equals(object obj) => obj is FourCorners<TValue> other && Equals(other);

        public override int GetHashCode() => BottomLeft.GetHashCode();

        public bool Equals(FourCorners<TValue> other)
        {
            if (this == other) return true;
            if (other == null) return false;
            return BottomLeft.Equals(other.BottomLeft) && BottomRight.Equals(other.BottomRight)
                             && TopLeft.Equals(other.TopLeft) && TopRight.Equals(other.TopRight);
        }

        public override string ToString()
        {
            if (IsOneValue) return BottomLeft.ToString();
            return "Four Corners";
        }

        private void refreshOneValue()
        {
            IsOneValue = BottomLeft.Equals(BottomRight) && BottomRight.Equals(TopLeft) && TopLeft.Equals(TopRight);
        }
    }
}