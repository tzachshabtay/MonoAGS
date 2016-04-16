using System;
using AGS.API;

namespace AGS.Engine
{
	public struct AGSSquare : ISquare
	{
		public AGSSquare (PointF bottomLeft, PointF bottomRight, PointF topLeft, PointF topRight) : this()
		{
			BottomLeft = bottomLeft;
			BottomRight = bottomRight;
			TopLeft = topLeft;
			TopRight = topRight;

			MinX = MathUtils.Min(bottomLeft.X, bottomRight.X, topLeft.X, topRight.X);
			MaxX = MathUtils.Max(bottomLeft.X, bottomRight.X, topLeft.X, topRight.X);
			MinY = MathUtils.Min(bottomLeft.Y, bottomRight.Y, topLeft.Y, topRight.Y);
			MaxY = MathUtils.Max(bottomLeft.Y, bottomRight.Y, topLeft.Y, topRight.Y);
		}

		#region ISquare implementation

		//http://www.emanueleferonato.com/2012/03/09/algorithm-to-determine-if-a-point-is-inside-a-square-with-mathematics-no-hit-test-involved/
		public bool Contains (PointF p)
		{
			PointF a = BottomLeft;
			PointF b = BottomRight;
			PointF c = TopRight;
			PointF d = TopLeft;

			if (triangleArea(a,b,p)>0 || triangleArea(b,c,p)>0 || 
				triangleArea(c,d,p)>0 || triangleArea(d,a,p)>0) 
			{
				return false;
			}
			return true;
		}

		public PointF BottomLeft { get; private set; }

		public PointF BottomRight { get; private set; }

		public PointF TopLeft { get; private set; }

		public PointF TopRight { get; private set; }

		public float MinX { get; private set; }
		public float MaxX { get; private set; }

		public float MinY { get; private set; }
		public float MaxY { get; private set; }

		#endregion

		public override string ToString ()
		{
			return string.Format ("[A={0}, B={1}, C={2}, D={3}]", BottomLeft, BottomRight, TopLeft, TopRight);
		}

		private float triangleArea(PointF a,PointF b,PointF c) 
		{
			return (c.X * b.Y - b.X * c.Y) - (c.X * a.Y - a.X * c.Y) + (b.X * a.Y - a.X * b.Y);
		}
	}
}

