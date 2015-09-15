using System;
using OpenTK;
using AGS.API;

namespace AGS.Engine
{
	public interface IGLBoundingBoxBuilder
	{
		IGLBoundingBoxes Build(float width, float height, IGLMatrices matrices);
	}
}

