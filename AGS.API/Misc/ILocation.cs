namespace AGS.API
{
    public interface ILocation
	{
		float X { get; }
		float Y { get; }
	    float Z { get; }

		PointF XY { get; }
	}
}

