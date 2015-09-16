using System;

namespace AGS.API
{
	public interface IArea
	{
		IMask Mask { get; set; }
		bool Enabled { get; set; }

		bool IsInArea(IPoint point);
		bool IsInArea(IPoint point, ISquare projectionBox, float scaleX, float scaleY);
		IPoint FindClosestPoint(IPoint point, out float distance);
	}
}

