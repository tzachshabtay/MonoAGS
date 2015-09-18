using System;

namespace AGS.Engine
{
	public class GLBoundingBoxes : IGLBoundingBoxes
	{
		public GLBoundingBoxes(IGLBoundingBox renderBox, IGLBoundingBox hitTestBox)
		{
			RenderBox = renderBox;
			HitTestBox = hitTestBox;
		}

		#region IGLBoundingBoxes implementation

		public IGLBoundingBox RenderBox { get; private set; }

		public IGLBoundingBox HitTestBox { get; private set; }

		#endregion
	}
}

