using System;
using API;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Engine
{
	public class GLImage : IImage
	{
		public GLImage ()
		{
			Width = 1f;
			Height = 1f;
			ID = "";
		}

		public GLImage(Bitmap bitmap, string id, int texture)
		{
			OriginalBitmap = bitmap;
			Width = bitmap.Width;
			Height = bitmap.Height;
			ID = id;
			Texture = texture;
		}

		public Bitmap OriginalBitmap { get; private set; }

		public float Width { get; private set; }
		public float Height { get; private set; }

		public string ID { get; private set; }
		public int Texture { get; private set; }

		public override string ToString()
		{
			return ID ?? base.ToString();
		}
	}
}

