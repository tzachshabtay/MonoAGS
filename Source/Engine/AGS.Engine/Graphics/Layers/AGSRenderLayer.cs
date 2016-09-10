using AGS.API;

namespace AGS.Engine
{
	public class AGSRenderLayer : IRenderLayer
	{
		public AGSRenderLayer(int z, PointF? parallaxSpeed = null, Size? independentResolution = null)
		{
			Z = z;
            ParallaxSpeed = parallaxSpeed ?? new PointF(1f, 1f);
            IndependentResolution = independentResolution;
		}

		#region IRenderLayer implementation

		public int Z { get; private set; }

		public PointF ParallaxSpeed { get; private set; }

        public Size? IndependentResolution { get; private set; }

		#endregion
	}
}

