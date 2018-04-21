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
        private static List<Action<Resolver>> _overrides = new List<Action<Resolver>>();

		public Resolver(IDevice device)
		{
			Builder = new ContainerBuilder ();

            if (device.ConfigFile.DebugResolves)
			{
				Builder.RegisterModule(new AutofacResolveLoggingModule ());
			}

			Builder.RegisterAssemblyTypes(typeof(AGSGame).GetTypeInfo().Assembly).
                   Except<SpatialAStarPathFinder>().AsImplementedInterfaces().ExternallyOwned();

            registerDevice(device);

            RegisterType<AGSObject, IObject>();
            RegisterType<GLImage, IImage>();
            RegisterType<AGSDialogActions, IDialogActions>();
            RegisterType<AGSSayLocationProvider, ISayLocationProvider>();
            RegisterType<AGSTreeNodeViewProvider, ITreeNodeViewProvider>();

			Builder.RegisterType<AGSGameState>().SingleInstance().As<IGameState>();
			Builder.RegisterType<AGSGame>().SingleInstance().As<IGame>();
			Builder.RegisterType<AGSGameEvents>().SingleInstance().As<IGameEvents>();
			Builder.RegisterType<BitmapPool>().SingleInstance();
			Builder.RegisterType<ResourceLoader>().SingleInstance().As<IResourceLoader>();
			Builder.RegisterType<AGSCutscene>().SingleInstance().As<ICutscene>();
			Builder.RegisterType<AGSRoomTransitions>().SingleInstance().As<IAGSRoomTransitions>();
			Builder.RegisterType<ALAudioSystem>().SingleInstance().As<IAudioSystem>();
			Builder.RegisterType<RoomMusicCrossFading>().SingleInstance().As<ICrossFading>();
			Builder.RegisterType<AGSAudioSettings>().SingleInstance().As<IAudioSettings>();
			Builder.RegisterType<ALListener>().SingleInstance().As<IAudioListener>();
            Builder.RegisterType<RenderThreadSyncContext>().SingleInstance().As<IRenderMessagePump>().As<IRenderThread>();
            Builder.RegisterType<UpdateThreadSyncContext>().SingleInstance().As<IUpdateMessagePump>().As<IUpdateThread>();
            Builder.RegisterType<AGSClassicSpeechCache>().SingleInstance().As<ISpeechCache>();
            Builder.RegisterType<GLGraphicsFactory>().SingleInstance().As<IGraphicsFactory>();
            Builder.RegisterType<GLUtils>().SingleInstance().As<IGLUtils>();
            Builder.RegisterType<AGSFocusedUI>().SingleInstance().As<IFocusedUI>().As<IModalWindows>();
            Builder.RegisterType<RoomLimitsFromBackground>().SingleInstance().As<IRoomLimitsProvider>();
            Builder.RegisterType<UIEventsAggregator>().SingleInstance();
            Builder.RegisterType<AGSDisplayList>().SingleInstance().As<IDisplayList>();
            Builder.RegisterType<AGSHitTest>().SingleInstance().As<IHitTest>();
            Builder.RegisterType<GLTextureCache>().SingleInstance().As<ITextureCache>();
            Builder.RegisterType<AGSDefaultInteractions>().SingleInstance().As<IDefaultInteractions>();
            Builder.RegisterType<InventorySubscriptions>().SingleInstance();
            Builder.RegisterType<AGSShouldBlockInput>().SingleInstance().As<IShouldBlockInput>();

			registerComponents();

			RegisterType<AGSSprite, ISprite>();
            RegisterType<AGSBoundingBoxesBuilder, IBoundingBoxBuilder>();
            RegisterType<AGSTranslate, ITranslate>();
            RegisterType<AGSScale, IScale>();
            RegisterType<AGSRotate, IRotate>();
            RegisterType<AGSHasImage, IHasImage>();
            RegisterType<AGSEvent, IEvent>();
            RegisterType<AGSEvent, IBlockingEvent>();
            RegisterType<AGSRestrictionList, IRestrictionList>();

            Builder.RegisterGeneric(typeof(AGSEvent<>)).As(typeof(IEvent<>)).ExternallyOwned();
            Builder.RegisterGeneric(typeof(AGSEvent<>)).As(typeof(IBlockingEvent<>)).ExternallyOwned();

			FastFingerChecker checker = new FastFingerChecker ();
			Builder.RegisterInstance(checker);

            Builder.RegisterSource(new ResolveAnythingSource());

            foreach (var action in _overrides) action(this);
		}

        public void RegisterType<TType, TInterface>()
        {
            Builder.RegisterType<TType>().As<TInterface>().ExternallyOwned();
        }

        public static void Override(Action<Resolver> action)
        {
            _overrides.Add(action);
        }

		public ContainerBuilder Builder { get; private set; }

		public IContainer Container { get; private set; }

		public void Build()
		{
			Builder.RegisterInstance(this);
            Container = Builder.Build();
		}

        private void registerDevice(IDevice device)
        {
            Builder.RegisterInstance(device).As<IDevice>();
            Builder.RegisterInstance(device.Assemblies).As<IAssemblies>();
            Builder.RegisterInstance(device.BitmapLoader).As<IBitmapLoader>();
            Builder.RegisterInstance(device.BrushLoader).As<IBrushLoader>();
            Builder.RegisterInstance(device.ConfigFile).As<IEngineConfigFile>();
            Builder.RegisterInstance(device.FileSystem).As<IFileSystem>();
            Builder.RegisterInstance(device.GraphicsBackend).As<IGraphicsBackend>();
            Builder.RegisterInstance(device.KeyboardState).As<IKeyboardState>();
        }

		private void registerComponents()
		{
			var assembly = typeof(Resolver).GetTypeInfo().Assembly;
			foreach (var type in assembly.DefinedTypes)
			{
				if (!isComponent(type)) continue;
				registerComponent(type);
			}
			RegisterType<VisibleProperty, IVisibleComponent>();
			RegisterType<EnabledProperty, IEnabledComponent>();
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
                Builder.RegisterType(type.AsType()).As(compInterface).ExternallyOwned();
			}
		}
	}
}