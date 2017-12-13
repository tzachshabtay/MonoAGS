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

        public float X => XY.X;
        public float Y => XY.Y;
        public float Z => _z.HasValue ? _z.Value : Y;
        public PointF XY => _xy;

        public override string ToString ()
		{
            return $"{XY.ToString()},{Z:0.##}";
		}
	}
}

