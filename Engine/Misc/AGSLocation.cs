using System;
using API;

namespace Engine
{
	public struct AGSLocation : ILocation
	{
		private IPoint point;
		private float? z;

		public AGSLocation() : this(new AGSPoint(), 0)
		{}

		public AGSLocation (IPoint point, float? z = null)
		{
			this.point = point;
			this.z = z;
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

