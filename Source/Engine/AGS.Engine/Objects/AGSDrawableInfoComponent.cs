using AGS.API;

namespace AGS.Engine
{
	public class AGSDrawableInfoComponent : AGSComponent, IDrawableInfoComponent
	{
        public IRenderLayer RenderLayer { get; set; }

        public bool IgnoreViewport { get; set; }

        public bool IgnoreScalingArea { get; set; }
	}
}

