using AGS.API;

namespace AGS.Engine
{
	public interface IBoundingBoxBuilder
	{
		PointF Build(AGSBoundingBoxes boxes, float width, float height, IGLMatrices matrices, bool buildRenderBox, bool buildHitTestBox);
        IEvent OnNewBoxBuildRequired { get; }
	}
}

