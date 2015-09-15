using System;
using AGS.API;

namespace AGS.Engine
{
	public struct AGSPoint : IPoint
	{
		public AGSPoint (float x, float y)
		{
			X = x;
			Y = y;
		}

		#region IPoint implementation

		public float X { get; private set; }

		public float Y { get; private set; }

		#endregion

		public override string ToString ()
		{
			return string.Format ("{0:0.##},{1:0.##}", X, Y);
		}

		public override bool Equals(Object obj) 
		{
			if (obj == null) return false;

			IPoint p = obj as IPoint;
			if (obj == null) return false;
			return (X == p.X) && (Y == p.Y);
		}

		public override int GetHashCode() 
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}

	}
}

