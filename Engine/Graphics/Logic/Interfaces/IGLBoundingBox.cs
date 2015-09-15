using System;
using OpenTK;

namespace AGS.Engine
{
	public interface IGLBoundingBox
	{
		Vector3 BottomLeft { get; }
		Vector3 TopLeft { get; }
		Vector3 BottomRight { get; }
		Vector3 TopRight { get; }
	}
}

