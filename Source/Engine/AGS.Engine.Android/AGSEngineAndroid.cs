using System;
using System.Diagnostics;
using System.Linq;
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

            Hooks.GraphicsBackend = new OpenGLESBackend();
			Hooks.BitmapLoader = new AndroidBitmapLoader (Hooks.GraphicsBackend);
			Hooks.BrushLoader = new AndroidBrushLoader ();
			Hooks.FontLoader = new AndroidFontLoader ();
			Hooks.ConfigFile = new AndroidEngineConfigFile ();
            Hooks.EntryAssembly = Assembly.GetEntryAssembly() ?? getAssembly();
			Hooks.FileSystem = new AndroidFileSystem ();

		    Resolver.Override(resolver => resolver.Builder.RegisterType<AndroidGameWindowSize>().SingleInstance().As<IGameWindowSize>());
            //Resolver.Override(resolver => resolver.Builder.RegisterType<AGSGameWindow>().SingleInstance().As<IGameWindow>());
            Resolver.Override(resolver => resolver.Builder.RegisterType<ALAudioBackend>().SingleInstance().As<IAudioBackend>());
        }

        private static Assembly getAssembly()
        { 
            var methodFrames = new StackTrace().GetFrames().Select(t => t.GetMethod()).ToArray();
            foreach (var frame in methodFrames)
            {
                if (frame.Name == "OnCreate") return frame.Module.Assembly;
            }
            return Assembly.GetAssembly(typeof(AGSEngineAndroid));
        }
	}
}

