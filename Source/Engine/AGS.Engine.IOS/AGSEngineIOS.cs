using System;
using Autofac;

namespace AGS.Engine.IOS
{
    public class AGSEngineIOS
    {
        private static IOSAssemblies _assembly;

        public static void Init()
        {
            //OpenTK.Toolkit.Init();

            var device = new IOSDevice(_assembly);
            AGSGame.Device = device;

            //Resolver.Override(resolver => resolver.Builder.RegisterType<AndroidGameWindowSize>().SingleInstance().As<IGameWindowSize>());
            Resolver.Override(resolver => resolver.Builder.RegisterType<ALAudioBackend>().SingleInstance().As<IAudioBackend>());
        }

        public static void SetAssembly()
        {
            _assembly = new IOSAssemblies();
        }
    }
}
