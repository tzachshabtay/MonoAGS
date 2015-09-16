using System;
using AGS.API;

namespace AGS.Engine
{
	public struct AGSSquare : ISquare
	{
		public AGSSquare (IPoint bottomLeft, IPoint bottomRight, IPoint topLeft, IPoint topRight) : this()
		{
			BottomLeft = bottomLeft;
			BottomRight = bottomRight;
			TopLeft = topLeft;
			TopRight = topRight;
		}

		#region ISquare implementation

		//http://www.emanueleferonato.com/2012/03/09/algorithm-to-determine-if-a-point-is-inside-a-square-with-mathematics-no-hit-test-involved/
		public bool Contains (IPoint p)
		{
			IPoint a = BottomLeft;
			IPoint b = BottomRight;
			IPoint c = TopRight;
			IPoint d = TopLeft;

			if (triangleArea(a,b,p)>0 || triangleArea(b,c,p)>0 || 
				triangleArea(c,d,p)>0 || triangleArea(d,a,p)>0) 
			{
				return false;
			}
			return true;
		}

		public IPoint BottomLeft { get; private set; }

		public IPoint BottomRight { get; private set; }

		public IPoint TopLeft { get; private set; }

		public IPoint TopRight { get; private set; }

		#endregion

		public override string ToString ()
		{
			return string.Format ("[A={0}, B={1}, C={2}, D={3}]", BottomLeft, BottomRight, TopLeft, TopRight);
		}

		private float triangleArea(IPoint a,IPoint b,IPoint c) 
		{
			return (c.X * b.Y - b.X * c.Y) - (c.X * a.Y - a.X * c.Y) + (b.X * a.Y - a.X * b.Y);
		}
	}
}

