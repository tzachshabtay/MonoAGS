using AGS.Engine.Desktop;
using Autofac;

namespace AGS.Engine.Android
{
	public class AGSEngineAndroid
	{
        private static AndroidAssemblies _assembly;

        public static void Init()
		{
            OpenTK.Toolkit.Init();

            var device = new AndroidDevice(_assembly);
            AGSGame.Device = device;

		    Resolver.Override(resolver => resolver.Builder.RegisterType<AndroidGameWindowSize>().SingleInstance().As<IGameWindowSize>());
            Resolver.Override(resolver => resolver.Builder.RegisterType<ALAudioBackend>().SingleInstance().As<IAudioBackend>());
        }

        public static void SetAssembly()
        {
            _assembly = new AndroidAssemblies();
        }
	}
}

