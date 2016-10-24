using AGS.API;

namespace AGS.Engine
{
	public interface IGLMatrixBuilder
	{
		IGLMatrices Build(IHasModelMatrix obj, IHasModelMatrix sprite, IObject parent, Matrix4 viewport, PointF areaScaling, PointF resolutionTransform);
	}
}

