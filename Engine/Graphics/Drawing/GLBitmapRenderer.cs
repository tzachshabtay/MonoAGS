using System;
using AGS.API;
using System.Drawing;

namespace AGS.Engine
{
	public class GLBitmapRenderer : IImageRenderer
	{
		GLImage glImage;

		public GLBitmapRenderer (GLGraphicsFactory factory, IBitmap bitmap)
		{
			glImage = (GLImage)factory.LoadImage (bitmap);
			Opacity = 0.5f;
		}

		#region ICustomRenderer implementation

		public void Prepare(IObject obj, IViewport viewport, IPoint areaScaling)
		{
		}

		public void Render (IObject obj, IViewport viewport, IPoint areaScaling)
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

