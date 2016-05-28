﻿using System;
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
			Builder.RegisterType<AGSSayLocation>().As<ISayLocation>();

			Builder.RegisterType<AGSGameState>().SingleInstance().As<IGameState>();
			Builder.RegisterType<AGSGame>().SingleInstance().As<IGame>();
			Builder.RegisterType<AGSGameEvents>().SingleInstance().As<IGameEvents>();
			Builder.RegisterType<BitmapPool>().SingleInstance();
			Builder.RegisterType<GLViewportMatrixFactory>().SingleInstance().As<IGLViewportMatrixFactory>();
			Builder.RegisterType<AGSPlayer>().SingleInstance().As<IPlayer>();
			Builder.RegisterType<ResourceLoader>().SingleInstance().As<IResourceLoader>();
			Builder.RegisterType<AGSCutscene>().SingleInstance().As<ICutscene>();
			Builder.RegisterType<AGSRoomTransitions>().SingleInstance().As<IAGSRoomTransitions>();

			registerComponents();

			Builder.RegisterType<AGSSprite>().As<ISprite>();
            Builder.RegisterType<GLMatrixBuilder>().As<IGLMatrixBuilder>();
            Builder.RegisterType<GLBoundingBoxesBuilder>().As<IGLBoundingBoxBuilder>();
			Builder.RegisterGeneric(typeof(AGSEvent<>)).As(typeof(IEvent<>));
			Builder.RegisterGeneric(typeof(AGSEvent<>)).As(typeof(IBlockingEvent<>));

			Dictionary<string, GLImage> textures = new Dictionary<string, GLImage> (1024);
			Builder.RegisterInstance(textures);
			Builder.RegisterInstance(textures).As(typeof(IDictionary<string, GLImage>));

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

