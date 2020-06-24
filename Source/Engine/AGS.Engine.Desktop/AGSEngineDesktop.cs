using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using Autofac;
using AGS.API;

namespace AGS.Engine.Desktop
{
	public static class AGSEngineDesktop
    {
		public static void Init()
		{                        
			OpenTK.Toolkit.Init();
            OpenALSoftLoader.Load();
		    string currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		    Trace.Assert(currentDir != null);
			currentDir = currentDir.Replace(".Desktop", "");
			Directory.CreateDirectory(currentDir);
			Environment.CurrentDirectory = currentDir;

            DesktopDevice device = new DesktopDevice();
            AGSGame.Device = device;

            Resolver.Override(resolver => resolver.Builder.RegisterType<AGSInput>().SingleInstance().As<IInput>().As<AGSInput>());
            Resolver.Override(resolver => resolver.Builder.RegisterType<DesktopGameWindowSize>().SingleInstance().As<IGameWindowSize>());
            Resolver.Override(resolver => resolver.Builder.RegisterType<AGSGameWindow>().SingleInstance().As<IGameWindow>().As<IWindowInfo>());
            Resolver.Override(resolver => resolver.Builder.RegisterType<ALAudioBackend>().SingleInstance().As<IAudioBackend>());
            Resolver.Override(resolver => resolver.Builder.Register(_ => AGSGameWindow.GameWindow).As<OpenTK.INativeWindow>());
        }
	}
}
