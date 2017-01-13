using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AGS.Engine.Desktop;
using Android.Content.Res;
using Autofac;

namespace AGS.Engine.Android
{
	public class AGSEngineAndroid
	{
        private static Assembly _assembly;

        public static void Init()
		{
            OpenTK.Toolkit.Init();

            Hooks.DisplayDensity = Resources.System.DisplayMetrics.Density;
            Hooks.GraphicsBackend = new OpenGLESBackend();
			Hooks.BitmapLoader = new AndroidBitmapLoader (Hooks.GraphicsBackend);
			Hooks.BrushLoader = new AndroidBrushLoader ();
			Hooks.FontLoader = new AndroidFontLoader ();
			Hooks.ConfigFile = new AndroidEngineConfigFile ();
            Hooks.EntryAssembly = _assembly; //Assembly.GetEntryAssembly() ?? getAssembly();
			Hooks.FileSystem = new AndroidFileSystem ();

		    Resolver.Override(resolver => resolver.Builder.RegisterType<AndroidGameWindowSize>().SingleInstance().As<IGameWindowSize>());
            //Resolver.Override(resolver => resolver.Builder.RegisterType<AGSGameWindow>().SingleInstance().As<IGameWindow>());
            Resolver.Override(resolver => resolver.Builder.RegisterType<ALAudioBackend>().SingleInstance().As<IAudioBackend>());
        }

        public static void SetAssembly()
        {
            _assembly = getAssembly();
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

