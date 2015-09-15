using System;

namespace AGS.Engine
{
	public interface IGLBoundingBoxes
	{
		IGLBoundingBox RenderBox { get; }
		IGLBoundingBox HitTestBox { get; }
	}
}

