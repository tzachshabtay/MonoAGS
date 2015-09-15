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
		public Resolver()
		{
			Builder = new ContainerBuilder ();
			Builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).
				Except<SpatialAStarPathFinder>().AsImplementedInterfaces();

			Builder.RegisterType<GLImageRenderer>().As<IImageRenderer>();
			Builder.RegisterType<AGSObject>().As<IObject>();
			Builder.RegisterType<AGSSprite>().As<ISprite>();

			Builder.RegisterType<AGSGameState>().SingleInstance().As<IGameState>();
			Builder.RegisterType<AGSGame>().SingleInstance().As<IGame>();
			Builder.RegisterType<AGSGameEvents>().SingleInstance().As<IGameEvents>();
			Builder.RegisterType<BitmapPool>().SingleInstance();

			Builder.RegisterGeneric(typeof(AGSEvent<>)).As(typeof(IEvent<>));

			Dictionary<string, GLImage> textures = new Dictionary<string, GLImage> (1024);
			Builder.RegisterInstance(textures);

			Builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
		}

		public ContainerBuilder Builder { get; private set; }

		public IContainer Container { get; private set; }

		public void Build()
		{
			Container = Builder.Build();

			var updater = new ContainerBuilder ();
			updater.RegisterInstance(Container);
			updater.Update(Container);
		}
	}
}

