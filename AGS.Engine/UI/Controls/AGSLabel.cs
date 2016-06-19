using System;
using AGS.API;

namespace AGS.Engine
{
	public partial class AGSLabel
	{
		partial void init(Resolver resolver)
		{
			RenderLayer = AGSLayers.UI;
			IgnoreScalingArea = true;
			IgnoreViewport = true;
			Anchor = new PointF ();

			Enabled = false;
		}

		public void ApplySkin(ILabel label)
		{
			throw new NotSupportedException ();
		}
	}
}

