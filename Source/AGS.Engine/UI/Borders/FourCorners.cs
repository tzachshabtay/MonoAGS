using System;

namespace AGS.Engine
{
	public class FourCorners<TValue>
	{
		public FourCorners(TValue value) : this(value,value,value,value)
		{
		}

		public FourCorners(TValue bottomLeft, TValue bottomRight, TValue topLeft, TValue topRight)
		{
			BottomLeft = bottomLeft;
			BottomRight = bottomRight;
			TopLeft = topLeft;
			TopRight = topRight;
		}

		public TValue BottomLeft { get; set; }
		public TValue BottomRight { get; set; }
		public TValue TopLeft { get; set; }
		public TValue TopRight { get; set; }

		public FourCorners<TNewValue> Convert<TNewValue>(Func<TValue, TNewValue> convert)
		{
			return new FourCorners<TNewValue> (convert(BottomLeft), convert(BottomRight), convert(TopLeft),
				convert(TopRight));
		}
	}
}

