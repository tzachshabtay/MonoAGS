using System;
using System.Reflection;
using System.IO;

namespace AGS.Engine.Desktop
{
	public static class AGSEngineDesktop
	{
		private static DesktopFontFamilyLoader _fontFamilyLoader; //Must stay in memory

		public static void Init()
		{                        
			OpenTK.Toolkit.Init();
			string currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace(".Desktop", "");
			Directory.CreateDirectory(currentDir);
			Environment.CurrentDirectory = currentDir;
			Hooks.EntryAssembly = Assembly.GetEntryAssembly();
			_fontFamilyLoader = new DesktopFontFamilyLoader(new ResourceLoader());

            Hooks.GraphicsBackend = new GLGraphicsBackend();
			Hooks.BitmapLoader = new DesktopBitmapLoader (Hooks.GraphicsBackend);
			Hooks.BrushLoader = new DesktopBrushLoader ();
			Hooks.FontLoader = new DesktopFontLoader (_fontFamilyLoader);
			Hooks.GameWindowSize = new DesktopGameWindowSize ();
			Hooks.ConfigFile = new DesktopEngineConfigFile ();
			Hooks.FileSystem = new DesktopFileSystem ();
            Hooks.KeyboardState = new DesktopKeyboardState();
		}
	}
}

