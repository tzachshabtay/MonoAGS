using System;

namespace AGS.API
{
	public struct PointF
	{
		private readonly float _x, _y;

		public PointF(float x, float y)
		{
			_x = x;
			_y = y;
		}

		public float X { get { return _x; } }
		public float Y { get { return _y; } }

		public override string ToString ()
		{
			return string.Format ("{0:0.##},{1:0.##}", X, Y);
		}

		public override bool Equals(Object obj) 
		{
			if (obj == null) return false;
			if (!(obj is PointF)) return false;

			PointF p = (PointF)obj;
			return (X == p.X) && (Y == p.Y);
		}

		public override int GetHashCode() 
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}
	}
}

