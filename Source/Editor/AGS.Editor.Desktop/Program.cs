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
            AGSEditor.SetupResolver();
            AGSEngineDesktop.Init();
            AGSEditor.Platform = new DesktopEditorPlatform();
            Program.Run();
		}
	}
}