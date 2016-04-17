using System;
using System.Reflection;
using System.IO;

namespace AGS.Engine.Desktop
{
	public static class AGSEngineDesktop
	{
		public static void Init()
		{
			OpenTK.Toolkit.Init();
			Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			Hooks.BitmapLoader = new DesktopBitmapLoader ();
			Hooks.BrushLoader = new DesktopBrushLoader ();
			Hooks.FontLoader = new DesktopFontLoader ();
			Hooks.GameWindowSize = new DesktopGameWindowSize ();
			Hooks.ConfigFile = new DesktopEngineConfigFile ();
			Hooks.EntryAssembly = Assembly.GetEntryAssembly();
			Hooks.FileSystem = new DesktopFileSystem ();
		}
	}
}

