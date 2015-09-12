using System;
using OpenTK;

namespace Engine
{
	public interface IGLBoundingBoxBuilder
	{
		IGLBoundingBox Build(float width, float height, Matrix4 matrix);
	}
}

