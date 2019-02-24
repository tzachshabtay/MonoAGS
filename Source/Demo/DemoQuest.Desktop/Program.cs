using AGS.Engine.Desktop;
using DemoGame;

namespace DemoQuest.Desktop
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			AGSEngineDesktop.Init();
			DemoStarter.Run();
		}
	}
}
