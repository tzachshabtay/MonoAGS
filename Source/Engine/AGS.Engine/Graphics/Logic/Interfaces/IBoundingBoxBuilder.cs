using AGS.API;

namespace AGS.Engine
{
	public interface IBoundingBoxBuilder
	{
        AGSBoundingBox BuildIntermediateBox(float width, float height, Matrix4 modelMatrix);
        AGSBoundingBox BuildHitTestBox(AGSBoundingBox intermediateBox);
        AGSBoundingBox BuildRenderBox(AGSBoundingBox intermediateBox, Matrix4 viewportMatrix, out PointF scale);
        IEvent OnNewBoxBuildRequired { get; }
	}
}

