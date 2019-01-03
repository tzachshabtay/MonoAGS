using AGS.API;


namespace AGS.Engine
{
    [PropertyFolder]
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

        public IBitmap OriginalBitmap => null;

        public float Width { get; }

		public float Height { get; }

        public string ID => "";

        public ISpriteSheet SpriteSheet => null;

        public ILoadImageConfig LoadConfig => null;

        public ITexture Texture => null;

        #endregion
    }
}

