using System;
using AGS.API;


namespace AGS.Engine
{
	public partial class AGSButton
	{
		partial void init(Resolver resolver)
		{
			RenderLayer = AGSLayers.UI;
			IgnoreScalingArea = true;
			IgnoreViewport = true;
			Anchor = new PointF ();

			Enabled = true;
		}

		public void ApplySkin(IButton button)
		{
			throw new NotSupportedException ();
		}
	}
}

