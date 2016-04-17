using System;
using AGS.API;

namespace AGS.Engine
{
	public struct AGSLocation : ILocation
	{
		private float? z;

		public AGSLocation (PointF point, float? z = null)
		{
			this.XY = point;
			this.z = z;
			if (z != null && z.Value == point.Y) this.z = null;
		}

		public AGSLocation(float x, float y, float? z = null) : this(new PointF(x,y), z)
		{}

		public static AGSLocation Empty()
		{
			return new AGSLocation (new PointF (), null);
		}

		public float X { get { return XY.X; } }
		public float Y { get { return XY.Y; } }
		public float Z { get { return z.HasValue ? z.Value : Y; } }
		public PointF XY { get; private set; }

		public override string ToString ()
		{
			return string.Format ("{0},{1}", XY.ToString(), Z);
		}
	}
}

