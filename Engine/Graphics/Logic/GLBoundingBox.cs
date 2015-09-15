using System;
using OpenTK;

namespace AGS.Engine
{
	public class GLBoundingBox : IGLBoundingBox
	{
		public GLBoundingBox()
		{
		}

		#region IGLBoundingBox implementation

		public Vector3 BottomLeft { get; set; }

		public Vector3 TopLeft { get; set; }

		public Vector3 BottomRight { get; set; }

		public Vector3 TopRight { get; set; }

		#endregion
	}
}

