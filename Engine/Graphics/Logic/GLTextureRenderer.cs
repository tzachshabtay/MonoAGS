using System;
using OpenTK;

namespace Engine
{
	public class GLTextureRenderer : IGLTextureRenderer
	{
		public GLTextureRenderer()
		{
		}

		#region IGLTextureRenderer implementation

		public void Render(int texture, IGLBoundingBox boundingBox, IGLColor color)
		{
			Vector3 bottomLeft = boundingBox.BottomLeft;
			Vector3 topLeft = boundingBox.TopLeft;
			Vector3 bottomRight = boundingBox.BottomRight;
			Vector3 topRight = boundingBox.TopRight;

			GLUtils.DrawQuad (texture, bottomLeft, bottomRight, topLeft, topRight, color.R,
				color.G, color.B, color.A);
		}

		#endregion
	}
}

