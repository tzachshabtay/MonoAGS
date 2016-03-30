using System;

namespace AGS.Engine
{
	public partial class AGSObject
	{
		partial void init(Resolver resolver)
		{
			RenderLayer = AGSLayers.Foreground;
			IgnoreScalingArea = true;
		}
	}
}

