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

		public bool IsInArea(PointF point)
		{
			return _area.IsInArea(point);
		}

		public bool IsInArea(PointF point, ISquare projectionBox, float scaleX, float scaleY)
		{
			return _area.IsInArea(point, projectionBox, scaleX, scaleY);
		}

		public PointF? FindClosestPoint(PointF point, out float distance)
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

