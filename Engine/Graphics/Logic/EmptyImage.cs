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

		public IBitmap OriginalBitmap { get { return null; } }

		public float Width { get; private set; }

		public float Height { get; private set; }

		public string ID { get { return ""; } }

		public ISpriteSheet SpriteSheet { get { return null; } }

		public ILoadImageConfig LoadConfig { get { return null; } }

		#endregion
	}
}

