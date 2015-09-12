using System;
using API;
using System.Drawing;

namespace Engine
{
	public class GLBitmapRenderer : IImageRenderer
	{
		GLImage glImage;

		public GLBitmapRenderer (GLGraphicsFactory factory, Bitmap bitmap)
		{
			glImage = factory.LoadImage (bitmap);
			Opacity = 0.5f;
		}

		#region ICustomRenderer implementation

		public void Render (IObject obj, IViewport viewport)
		{
			GLUtils.DrawQuad (glImage.Texture, X, Y, glImage.Width, glImage.Height, 0f, 
				1f, 1f, 1f, Opacity);
		}

		public float X { get; set; }
		public float Y { get; set; }
		public float Opacity { get; set; }

		#endregion
	}
}

