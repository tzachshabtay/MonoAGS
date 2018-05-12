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

        public Resolver(IDevice device, IGameSettings settings)
		{
			Builder = new ContainerBuilder ();

            if (device.ConfigFile.DebugResolves)
			{
				Builder.RegisterModule(new AutofacResolveLoggingModule ());
			}

			Builder.RegisterAssemblyTypes(typeof(AGSGame).GetTypeInfo().Assembly).
                   Except<SpatialAStarPathFinder>().AsImplementedInterfaces().ExternallyOwned();

            registerDevice(device);

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
            Builder.RegisterType<ALAudioSystem>().SingleInstance().As<IALAudioSystem>();
            Builder.RegisterType<AGSAudioSystem>().SingleInstance().As<IAudioSystem>();
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

            //Registering lambdas for increasing performance
            Builder.Register<IPanel>((c, p) => new AGSPanel(p.TypedAs<string>(), this)).ExternallyOwned();
            Builder.Register<IButton>((c, p) => new AGSButton(p.TypedAs<string>(), this)).ExternallyOwned();
            Builder.Register<ILabel>((c, p) => new AGSLabel(p.TypedAs<string>(), this)).ExternallyOwned();
            Builder.Register<IObject>((c, p) => new AGSObject(p.TypedAs<string>(), this)).ExternallyOwned();
            Builder.Register<ISlider>((c, p) => new AGSSlider(p.TypedAs<string>(), this)).ExternallyOwned();
            Builder.Register<ITextBox>((c, p) => new AGSTextbox(p.TypedAs<string>(), this)).ExternallyOwned();
            Builder.Register<ICheckBox>((c, p) => new AGSCheckBox(p.TypedAs<string>(), this)).ExternallyOwned();
            Builder.Register<IComboBox>((c, p) => new AGSComboBox(p.TypedAs<string>(), this)).ExternallyOwned();
            Builder.Register<ITranslateComponent>((c, p) => new AGSTranslateComponent(c.Resolve<ITranslate>())).ExternallyOwned();
            Builder.Register<ITranslate>((c, p) => new AGSTranslate()).ExternallyOwned();
            Builder.Register<IScaleComponent>((c, p) => new AGSScaleComponent(c.Resolve<IScale>())).ExternallyOwned();
            Builder.Register<IScale>((c, p) => new AGSScale()).ExternallyOwned();
            Builder.Register<IRotateComponent>((c, p) => new AGSRotateComponent(c.Resolve<IRotate>())).ExternallyOwned();
            Builder.Register<IRotate>((c, p) => new AGSRotate()).ExternallyOwned();
            Builder.Register<IDrawableInfoComponent>((c, p) => new AGSDrawableInfoComponent()).ExternallyOwned();
            Builder.Register<IUIEvents>((c, p) => new AGSUIEvents(c.Resolve<UIEventsAggregator>())).ExternallyOwned();
            Builder.Register<ISkinComponent>((c, p) => new AGSSkinComponent(c.Resolve<IGameSettings>())).ExternallyOwned();
            Builder.Register<IHasRoomComponent>((c, p) => new HasRoomComponent(c.Resolve<IGameState>(), c.Resolve<IAGSRoomTransitions>())).ExternallyOwned();
            Builder.Register<IAnimationComponent>((c, p) => new AGSAnimationComponent()).ExternallyOwned();
            Builder.Register<IInObjectTreeComponent>((c, p) => new InObjectTreeComponent()).ExternallyOwned();
            Builder.Register<IColliderComponent>((c, p) => new AGSCollider(c.Resolve<IGameState>())).ExternallyOwned();
            Builder.Register<IVisibleComponent>((c, p) => new VisibleProperty()).ExternallyOwned();
            Builder.Register<IEnabledComponent>((c, p) => new EnabledProperty()).ExternallyOwned();
            Builder.Register<ICustomPropertiesComponent>((c, p) => new AGSCustomPropertiesComponent(c.Resolve<ICustomProperties>())).ExternallyOwned();
            Builder.Register<IShaderComponent>((c, p) => new AGSShaderComponent()).ExternallyOwned();
            Builder.Register<IBorderComponent>((c, p) => new AGSBorderComponent(c.Resolve<IRenderPipeline>())).ExternallyOwned();
            Builder.Register<IPixelPerfectComponent>((c, p) => new AGSPixelPerfectComponent()).ExternallyOwned();
            Builder.Register<IModelMatrixComponent>((c, p) => new AGSModelMatrixComponent(c.Resolve<IRuntimeSettings>())).ExternallyOwned();
            Builder.Register<IWorldPositionComponent>((c, p) => new AGSWorldPositionComponent()).ExternallyOwned();

			RegisterType<AGSSprite, ISprite>();
            RegisterType<AGSBoundingBoxesBuilder, IBoundingBoxBuilder>();
            RegisterType<AGSHasImage, IHasImage>();
            RegisterType<AGSEvent, IEvent>();
            RegisterType<AGSEvent, IBlockingEvent>();
            RegisterType<AGSRestrictionList, IRestrictionList>();

            Builder.RegisterGeneric(typeof(AGSEvent<>)).As(typeof(IEvent<>)).ExternallyOwned();
            Builder.RegisterGeneric(typeof(AGSEvent<>)).As(typeof(IBlockingEvent<>)).ExternallyOwned();

			FastFingerChecker checker = new FastFingerChecker ();
			Builder.RegisterInstance(checker);
            Builder.RegisterInstance(settings).As<IGameSettings>();

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