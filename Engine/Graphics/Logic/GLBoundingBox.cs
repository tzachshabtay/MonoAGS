using System;
using OpenTK;

namespace Engine
{
	public class GLBoundingBox : IGLBoundingBox, IGLBoundingBoxBuilder
	{
		public GLBoundingBox()
		{
		}

		#region IGLBoundingBox implementation

		public Vector3 BottomLeft { get; private set; }

		public Vector3 TopLeft { get; private set; }

		public Vector3 BottomRight { get; private set; }

		public Vector3 TopRight { get; private set; }

		#endregion

		#region IGLBoundingBoxBuilder implementation

		public IGLBoundingBox Build(float width, float height, Matrix4 mvMatrix)
		{
			float left = 0f;
			float right = width;
			float bottom = 0f;
			float top = height;
			BottomLeft = Vector3.Transform(new Vector3 (left, bottom, 0f), mvMatrix);
			TopLeft = Vector3.Transform(new Vector3 (left, top, 0f), mvMatrix);
			BottomRight = Vector3.Transform(new Vector3 (right, bottom, 0f), mvMatrix);
			TopRight = Vector3.Transform(new Vector3 (right, top, 0f), mvMatrix);

			return this;
		}

		#endregion
	}
}

