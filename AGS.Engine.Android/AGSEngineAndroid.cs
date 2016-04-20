using System;
using System.Reflection;

namespace AGS.Engine.Android
{
	public class AGSEngineAndroid
	{
		public static void Init()
		{
			OpenTK.Toolkit.Init();

			Hooks.BitmapLoader = new AndroidBitmapLoader ();
			Hooks.BrushLoader = new AndroidBrushLoader ();
			Hooks.FontLoader = new AndroidFontLoader ();
			Hooks.GameWindowSize = new AndroidGameWindowSize ();
			Hooks.ConfigFile = new AndroidEngineConfigFile ();
			Hooks.EntryAssembly = Assembly.GetEntryAssembly();
			Hooks.FileSystem = new AndroidFileSystem ();
		}
	}
}

