using System;
using Autofac;

namespace AGS.Engine.IOS
{
    public class AGSEngineIOS
    {
        private static IOSAssemblies _assembly;

        public static void Init()
        {
            // On IOS, when the mono linker is enabled (and it's enabled by default) it strips out all parts of
            // the framework which are not in use. The linker does not support reflection, however, therefore it
            // does not recognize the call to 'GetRuntimeMethod(nameof(Convert.ChangeType)' (which is called from
            // a method in this Autofac in ConstructorParameterBinding class, ConvertPrimitiveType method) 
            // as using the 'Convert.ChangeType' method and strips it away unless it's
            // explicitly used somewhere else, crashing Autofac if that method is called.
            // Therefore, by explicitly calling it here, we're playing nicely with the linker and making sure
            // Autofac works on IOS.
            Convert.ChangeType((object)1234f, typeof(float));

            OpenTK.Toolkit.Init();

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
