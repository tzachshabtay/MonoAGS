using System;
using OpenTK;

namespace AGS.Engine
{
	public interface IGLBoundingBoxBuilder
	{
		IGLBoundingBox Build(float width, float height, Matrix4 matrix);
	}
}

