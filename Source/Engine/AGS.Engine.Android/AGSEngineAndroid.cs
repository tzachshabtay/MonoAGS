using System;
using System.Reflection;
using AGS.Engine.Desktop;
using Autofac;

namespace AGS.Engine.Android
{
	public class AGSEngineAndroid
	{
		public static void Init()
		{
			OpenTK.Toolkit.Init();

            Hooks.GraphicsBackend = new GLGraphicsBackend();
			Hooks.BitmapLoader = new AndroidBitmapLoader (Hooks.GraphicsBackend);
			Hooks.BrushLoader = new AndroidBrushLoader ();
			Hooks.FontLoader = new AndroidFontLoader ();
			Hooks.ConfigFile = new AndroidEngineConfigFile ();
			Hooks.EntryAssembly = Assembly.GetEntryAssembly();
			Hooks.FileSystem = new AndroidFileSystem ();

		    Resolver.Override(resolver => resolver.Builder.RegisterType<AndroidGameWindowSize>().SingleInstance().As<IGameWindowSize>());
            Resolver.Override(resolver => resolver.Builder.RegisterType<AGSGameWindow>().SingleInstance().As<IGameWindow>());
        }
	}
}

