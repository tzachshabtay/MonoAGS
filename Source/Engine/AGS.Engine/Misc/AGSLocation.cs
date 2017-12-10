using System;
using AGS.API;

namespace AGS.Engine
{
	public struct AGSLocation : ILocation
	{
		private readonly float? _z;
		private readonly PointF _xy;

		public AGSLocation (PointF point, float? z = null)
		{
			_xy = point;
			_z = z;
			if (_z != null && _z.Value == point.Y) _z = null;
		}

		public AGSLocation(float x, float y, float? z = null) : this(new PointF(x,y), z)
		{}

		public static AGSLocation Empty()
		{
			return new AGSLocation (new PointF (), null);
		}

		public float X { get { return XY.X; } }
		public float Y { get { return XY.Y; } }
		public float Z { get { return _z.HasValue ? _z.Value : Y; } }
		public PointF XY { get { return _xy; } }

		public override string ToString ()
		{
            return $"{XY.ToString()},{Z:0.##}";
		}
	}
}

