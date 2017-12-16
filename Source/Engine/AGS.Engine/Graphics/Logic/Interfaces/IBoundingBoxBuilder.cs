using AGS.API;

namespace AGS.Engine
{
	public interface IBoundingBoxBuilder
	{
        AGSBoundingBox BuildIntermediateBox(float width, float height, ref Matrix4 modelMatrix);
        AGSBoundingBox BuildHitTestBox(ref AGSBoundingBox intermediateBox);
        AGSBoundingBox BuildRenderBox(ref AGSBoundingBox intermediateBox, ref Matrix4 viewportMatrix, out PointF scale);
        IBlockingEvent OnNewBoxBuildRequired { get; }
	}
}

