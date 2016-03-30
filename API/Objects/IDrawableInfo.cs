using System;

namespace AGS.API
{
	public interface IDrawableInfo : IComponent
	{
		IRenderLayer RenderLayer { get; set; }
		bool IgnoreViewport { get; set; }
		bool IgnoreScalingArea { get; set; }
	}
}

