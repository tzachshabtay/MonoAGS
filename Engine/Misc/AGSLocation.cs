using System;
using AGS.API;

namespace AGS.Engine
{
	public struct AGSLocation : ILocation
	{
		private IPoint point;
		private float? z;

		public AGSLocation() : this(new AGSPoint(), null)
		{}

		public AGSLocation (IPoint point, float? z = null)
		{
			this.point = point;
			this.z = z;
			if (z != null && z.Value == point.Y) this.z = null;
		}

		public AGSLocation(float x, float y, float? z = null) : this(new AGSPoint(x,y), z)
		{}

		public float X { get { return point.X; } }
		public float Y { get { return point.Y; } }
		public float Z 
		{ 
			get { return z.HasValue ? z.Value : Y; }
			set 
			{
				if (value == Y)
					z = null;
				else
					z = value;
			}
		}

		public override string ToString ()
		{
			return string.Format ("{0},{1}", point.ToString(), Z);
		}
	}
}

