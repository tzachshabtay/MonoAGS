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

		public float Width { get { return distance(BottomLeft, BottomRight); }}
		public float Height { get { return distance(BottomLeft, TopLeft); }}
			
		#endregion

		private float distance(Vector3 a, Vector3 b)
		{
			float dx = a.X - b.X;
			float dy = a.Y - b.Y;
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}
	}
}

