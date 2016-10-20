using System;
using Autofac;
using System.Reflection;
using AGS.API;
using System.Collections.Generic;
using Autofac.Features.ResolveAnything;

namespace AGS.Engine
{
	public class Resolver
	{
		public Resolver(IEngineConfigFile configFile)
		{
			Builder = new ContainerBuilder ();

			if (configFile.DebugResolves)
			{
				Builder.RegisterModule(new AutofacResolveLoggingModule ());
			}

			Builder.RegisterAssemblyTypes(typeof(AGSGame).GetTypeInfo().Assembly).
				Except<SpatialAStarPathFinder>().AsImplementedInterfaces();

			Builder.RegisterType<GLImageRenderer>().As<IImageRenderer>();
			Builder.RegisterType<AGSObject>().As<IObject>();
			Builder.RegisterType<GLImage>().As<IImage>();
			Builder.RegisterType<AGSDialogActions>().As<IDialogActions>();
			Builder.RegisterType<AGSSayLocationProvider>().As<ISayLocationProvider>();

			Builder.RegisterType<AGSGameState>().SingleInstance().As<IGameState>();
			Builder.RegisterType<AGSGame>().SingleInstance().As<IGame>();
			Builder.RegisterType<AGSGameEvents>().SingleInstance().As<IGameEvents>();
			Builder.RegisterType<BitmapPool>().SingleInstance();
			Builder.RegisterType<GLViewportMatrixFactory>().SingleInstance().As<IGLViewportMatrixFactory>();
			Builder.RegisterType<ResourceLoader>().SingleInstance().As<IResourceLoader>();
			Builder.RegisterType<AGSCutscene>().SingleInstance().As<ICutscene>();
			Builder.RegisterType<AGSRoomTransitions>().SingleInstance().As<IAGSRoomTransitions>();
			Builder.RegisterType<ALAudioSystem>().SingleInstance().As<IAudioSystem>();
			Builder.RegisterType<RoomMusicCrossFading>().SingleInstance().As<ICrossFading>();
			Builder.RegisterType<AGSAudioSettings>().SingleInstance().As<IAudioSettings>();
			Builder.RegisterType<ALListener>().SingleInstance().As<IAudioListener>();
			Builder.RegisterType<AGSSyncContext>().SingleInstance().As<IMessagePump>();
            Builder.RegisterType<AGSClassicSpeechCache>().SingleInstance().As<ISpeechCache>();
            Builder.RegisterType<GLGraphicsFactory>().SingleInstance().As<IGraphicsFactory>();

			registerComponents();

			Builder.RegisterType<AGSSprite>().As<ISprite>();
            Builder.RegisterType<GLMatrixBuilder>().As<IGLMatrixBuilder>();
            Builder.RegisterType<GLBoundingBoxesBuilder>().As<IGLBoundingBoxBuilder>();
            Builder.RegisterType<AGSPixelPerfectCollidable>().As<IPixelPerfectCollidable>();
            Builder.RegisterType<AGSTranslate>().As<ITranslate>();
            Builder.RegisterType<AGSScale>().As<IScale>();
            Builder.RegisterType<AGSRotate>().As<IRotate>();
            Builder.RegisterType<AGSHasImage>().As<IHasImage>();

			Builder.RegisterGeneric(typeof(AGSEvent<>)).As(typeof(IEvent<>));
			Builder.RegisterGeneric(typeof(AGSEvent<>)).As(typeof(IBlockingEvent<>));

            Dictionary<string, ITexture> textures = new Dictionary<string, ITexture> (1024);
			Builder.RegisterInstance(textures);
            Builder.RegisterInstance(textures).As(typeof(IDictionary<string, ITexture>));

			FastFingerChecker checker = new FastFingerChecker ();
			Builder.RegisterInstance(checker);

			Builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
		}

		public ContainerBuilder Builder { get; private set; }

		public IContainer Container { get; private set; }

		public void Build()
		{
			Container = Builder.Build();

			var updater = new ContainerBuilder ();
			updater.RegisterInstance(Container);
			updater.RegisterInstance(this);
			updater.Update(Container);
		}

		private void registerComponents()
		{
			var assembly = typeof(Resolver).GetTypeInfo().Assembly;
			foreach (var type in assembly.DefinedTypes)
			{
				if (!isComponent(type)) continue;
				registerComponent(type);
			}
			Builder.RegisterType<VisibleProperty>().As<IVisibleComponent>();
			Builder.RegisterType<EnabledProperty>().As<IEnabledComponent>();
		}

		private bool isComponent(TypeInfo type)
		{
			return (type.BaseType == typeof(AGSComponent));
		}

		private void registerComponent(TypeInfo type)
		{
			foreach (var compInterface in type.ImplementedInterfaces)
			{
				if (compInterface == typeof(IComponent) || compInterface == typeof(IDisposable)) continue;
				Builder.RegisterType(type.AsType()).As(compInterface);
			}
		}
	}
}

