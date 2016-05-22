using System;
using OpenTK;
using AGS.API;

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

		public ISquare ToSquare()
		{
			ISquare square = new AGSSquare (new PointF (BottomLeft.X, BottomLeft.Y), new PointF (BottomRight.X, BottomRight.Y),
				new PointF (TopLeft.X, TopLeft.Y), new PointF (TopRight.X, TopRight.Y));
			if (square.BottomLeft.X > square.BottomRight.X) square = square.FlipHorizontal();
			return square;
		}
			
		#endregion

		private float distance(Vector3 a, Vector3 b)
		{
			float dx = a.X - b.X;
			float dy = a.Y - b.Y;
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}
	}
}

