using System;
using API;

namespace Engine
{
	public static class AGSLayers
	{
		static AGSLayers()
		{
			Background = new AGSRenderLayer (-100);
			Foreground = new AGSRenderLayer (0);
			UI = new AGSRenderLayer (100);
		}

		public static IRenderLayer Background { get; private set; }
		public static IRenderLayer Foreground { get; private set; }
		public static IRenderLayer UI { get; private set; }
	}
}

