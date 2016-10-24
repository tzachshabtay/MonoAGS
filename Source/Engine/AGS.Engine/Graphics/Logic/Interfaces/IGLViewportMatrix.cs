using AGS.API;

namespace AGS.Engine
{
	public interface IGLViewportMatrix
	{
		Matrix4 GetMatrix(IViewport viewport, PointF parallaxSpeed);
	}
}

