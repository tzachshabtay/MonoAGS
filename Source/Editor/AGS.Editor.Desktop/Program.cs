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