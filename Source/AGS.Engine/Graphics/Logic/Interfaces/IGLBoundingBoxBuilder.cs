using System;
using OpenTK;
using AGS.API;

namespace AGS.Engine
{
	public interface IGLBoundingBoxBuilder
	{
		void Build(IGLBoundingBoxes boxes, float width, float height, IGLMatrices matrices);
	}
}

