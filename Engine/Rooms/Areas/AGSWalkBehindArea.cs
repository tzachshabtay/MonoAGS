using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSWalkBehindArea : IWalkBehindArea
	{
		private IArea _area;

		public AGSWalkBehindArea(IArea area)
		{
			_area = area;
		}

		#region IArea implementation

		public bool IsInArea(IPoint point)
		{
			return _area.IsInArea(point);
		}

		public IPoint FindClosestPoint(IPoint point, out float distance)
		{
			return _area.FindClosestPoint(point, out distance);
		}

		public IMask Mask { get { return _area.Mask; } set { _area.Mask = value; } }

		public bool Enabled { get { return _area.Enabled; } set { _area.Enabled = value; } }

		#endregion

		#region IWalkBehindArea implementation

		public float? Baseline { get; set; }

		#endregion
	}
}

