using AGS.API;

namespace AGS.Engine
{
	public interface IGLBoundingBoxBuilder
	{
		PointF Build(IGLBoundingBoxes boxes, float width, float height, IGLMatrices matrices, bool buildRenderBox, bool buildHitTestBox);
	}
}

