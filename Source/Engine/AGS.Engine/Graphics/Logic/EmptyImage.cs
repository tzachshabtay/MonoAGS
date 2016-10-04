using System;
using AGS.API;


namespace AGS.Engine
{
	public class EmptyImage : IImage
	{
		public EmptyImage(float width, float height)
		{
			Width = width;
			Height = height;
		}

        public EmptyImage(PointF size) : this (size.X, size.Y)
        { }

		#region IImage implementation

		public IBitmap OriginalBitmap { get { return null; } }

		public float Width { get; private set; }

		public float Height { get; private set; }

		public string ID { get { return ""; } }

		public ISpriteSheet SpriteSheet { get { return null; } }

		public ILoadImageConfig LoadConfig { get { return null; } }

        public ITexture Texture { get { return null; } }

		#endregion
	}
}

