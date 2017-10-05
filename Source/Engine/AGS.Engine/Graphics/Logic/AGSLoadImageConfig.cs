using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
	public class AGSLoadImageConfig : ILoadImageConfig
	{
        public AGSLoadImageConfig(Point? transparentColorSamplePoint = null, ITextureConfig config = null)
		{
            TransparentColorSamplePoint = transparentColorSamplePoint;
            TextureConfig = config ?? new AGSTextureConfig();
		}

		#region ILoadImageConfig implementation

		public Point? TransparentColorSamplePoint { get; private set; }

        public ITextureConfig TextureConfig { get; private set; }

		#endregion
	}
}

