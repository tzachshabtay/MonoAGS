using System;
using API;

namespace Engine
{
	public class GLLineRenderer : IImageRenderer
	{
		public GLLineRenderer (float x1, float y1, float x2, float y2)
		{
			X1 = x1;
			X2 = x2;
			Y1 = y1;
			Y2 = y2;
		}

		#region ICustomRenderer implementation

		public void Render (IObject obj, IViewport viewport)
		{
			GLUtils.DrawLine (X1, Y1, X2, Y2, 1f, 1f, 0f, 0f, 1f);
		}

		public float X1 { get; private set; }
		public float Y1 { get; private set; }
		public float X2 { get; private set; }
		public float Y2 { get; private set; }

		#endregion
	}
}

