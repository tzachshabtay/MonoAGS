using System;
using AGS.API;
using OpenTK;

namespace AGS.Engine
{
	public interface IGLViewportMatrix
	{
		Matrix4 GetMatrix(IViewport viewport, PointF parallaxSpeed);
	}
}

