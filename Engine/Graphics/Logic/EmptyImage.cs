using System;
using AGS.API;
using System.Drawing;

namespace AGS.Engine
{
	public class EmptyImage : IImage
	{
		public EmptyImage(float width, float height)
		{
			Width = width;
			Height = height;
		}

		#region IImage implementation

		public Bitmap OriginalBitmap { get { return null; } }

		public float Width { get; private set; }

		public float Height { get; private set; }

		public string ID { get { return ""; } }
		#endregion
	}
}

