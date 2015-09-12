using System;
using API;
using OpenTK.Graphics.OpenGL;

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

		public float Width { get; set; }
		public float Height { get; set; }

		public string ID { get; set; }
		public int Texture { get; set; }

		public override string ToString()
		{
			return ID ?? base.ToString();
		}
	}
}

