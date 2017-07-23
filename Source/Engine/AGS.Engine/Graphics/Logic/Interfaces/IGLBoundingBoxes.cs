using System;

namespace AGS.Engine
{
	public interface AGSBoundingBoxes
	{
		AGSBoundingBox RenderBox { get; }
		AGSBoundingBox HitTestBox { get; }
	}
}

