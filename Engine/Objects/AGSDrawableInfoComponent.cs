using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSDrawableInfoComponent : AGSComponent, IDrawableInfo
	{
		public AGSDrawableInfoComponent()
		{
		}

		public IRenderLayer RenderLayer { get; set; }

		public bool IgnoreViewport { get; set; }

		public bool IgnoreScalingArea { get; set; }
	}
}

