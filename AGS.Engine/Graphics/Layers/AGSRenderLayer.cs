using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSRenderLayer : IRenderLayer
	{
		public AGSRenderLayer(int z) : this(z, new PointF(1f,1f))
		{}

		public AGSRenderLayer(int z, PointF parallaxSpeed)
		{
			Z = z;
			ParallaxSpeed = parallaxSpeed;
		}

		#region IRenderLayer implementation

		public int Z { get; private set; }

		public PointF ParallaxSpeed { get; private set; }

		#endregion
	}
}

