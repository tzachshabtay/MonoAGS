namespace AGS.API
{
    public interface ISquare
	{
		//Note: the points are listed as bottom left, bottom right, etc, even though it's not accurate.
		//A square doesn't really have bottom left, it just has 4 points. 
		//We use these descriptors for convenience, since each square in our engine was converted from
		//a rectangle which does have bottom left, etc.
		//Also, the order of the points is important for the "Contains" calculation.
		IPoint BottomLeft { get; }
		IPoint BottomRight { get; }
		IPoint TopLeft { get; }
		IPoint TopRight { get; }

		float MinX { get; }
		float MaxX { get; }
		float MinY { get; }
		float MaxY { get; }

		bool Contains(IPoint point);
	}
}

