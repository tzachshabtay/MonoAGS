namespace AGS.API
{
    public interface IRenderLayer
	{
		int Z { get; }
		PointF ParallaxSpeed { get; }
	}
}

