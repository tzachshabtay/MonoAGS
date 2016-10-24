namespace AGS.Engine
{
	public interface IGLBoundingBoxBuilder
	{
		void Build(IGLBoundingBoxes boxes, float width, float height, IGLMatrices matrices, bool buildRenderBox, bool buildHitTestBox);
	}
}

