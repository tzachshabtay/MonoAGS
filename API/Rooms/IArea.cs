using System;

namespace API
{
	public interface IArea
	{
		bool[][] Mask { get; set; }
		bool Enabled { get; set; }

		bool IsInArea(IPoint point);
		IPoint FindClosestPoint(IPoint point, out float distance);
		void ApplyToMask(bool[][] targetMask);
	}
}

