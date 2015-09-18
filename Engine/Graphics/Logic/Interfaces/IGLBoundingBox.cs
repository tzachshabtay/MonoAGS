using System;
using OpenTK;

namespace AGS.Engine
{
	public interface IGLBoundingBox
	{
		Vector3 BottomLeft { get; set; }
		Vector3 TopLeft { get; set; }
		Vector3 BottomRight { get; set; }
		Vector3 TopRight { get; set; }
	}
}

