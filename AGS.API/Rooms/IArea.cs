namespace AGS.API
{
    public interface IArea
	{
		IMask Mask { get; set; }
		bool Enabled { get; set; }

		bool IsInArea(PointF point);
		bool IsInArea(PointF point, ISquare projectionBox, float scaleX, float scaleY);
		PointF? FindClosestPoint(PointF point, out float distance);
	}
}

