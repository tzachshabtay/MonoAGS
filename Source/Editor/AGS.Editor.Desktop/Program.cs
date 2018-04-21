using System;
using System.IO;
using System.Reflection;
using AGS.Editor;
using AGS.Engine.Desktop;

namespace AGS.Editor.Desktop
{
	class MainClass
	{
		public static void Main(string[] args)
		{
            GameLoader.SetupResolver();
            AGSEngineDesktop.Init();
            GameLoader.Platform = new DesktopEditorPlatform();
            Program.Run();
		}
	}
}
