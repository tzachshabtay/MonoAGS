using System;
using System.Reflection;
using System.IO;
using Autofac;

namespace AGS.Engine.Desktop
{
	public static class AGSEngineDesktop
    {
		public static void Init()
		{                        
			OpenTK.Toolkit.Init();
			string currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace(".Desktop", "");
			Directory.CreateDirectory(currentDir);
			Environment.CurrentDirectory = currentDir;

            DesktopDevice device = new DesktopDevice();
            AGSGame.Device = device;

            Resolver.Override(resolver => resolver.Builder.RegisterType<DesktopGameWindowSize>().SingleInstance().As<IGameWindowSize>());
            Resolver.Override(resolver => resolver.Builder.RegisterType<AGSGameWindow>().SingleInstance().As<IGameWindow>());
            Resolver.Override(resolver => resolver.Builder.RegisterType<ALAudioBackend>().SingleInstance().As<IAudioBackend>());
        }
	}
}

