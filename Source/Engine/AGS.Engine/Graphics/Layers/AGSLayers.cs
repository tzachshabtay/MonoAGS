using AGS.API;

namespace AGS.Engine
{
	public static class AGSLayers
	{
		static AGSLayers()
		{
			Background = new AGSRenderLayer (100);
			Foreground = new AGSRenderLayer (0);
			UI = new AGSRenderLayer (-100);
            Dialogs = new AGSRenderLayer(-200);
            Speech = new AGSRenderLayer (-300);
		}

		public static IRenderLayer Background { get; private set; }
		public static IRenderLayer Foreground { get; private set; }
		public static IRenderLayer UI { get; private set; }
        public static IRenderLayer Dialogs { get; private set; }
        public static IRenderLayer Speech { get; private set; }
	}
}
