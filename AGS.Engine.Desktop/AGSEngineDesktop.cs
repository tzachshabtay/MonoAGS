using System;

namespace AGS.Engine.Desktop
{
	public static class AGSEngineDesktop
	{
		public static void InitHooks()
		{
			OpenTK.Toolkit.Init();
			Hooks.BitmapLoader = new DesktopBitmapLoader ();
			Hooks.BrushLoader = new DesktopBrushLoader ();
			Hooks.FontLoader = new DesktopFontLoader ();
			Hooks.GameWindowSize = new DesktopGameWindowSize ();
		}
	}
}

