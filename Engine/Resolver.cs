using System;
using Autofac;
using System.Reflection;
using API;
using System.Collections.Generic;
using Autofac.Features.ResolveAnything;

namespace Engine
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
			Dictionary<string, GLImage> textures = new Dictionary<string, GLImage> ();
			Builder.RegisterInstance(textures);
			Builder.RegisterGeneric(typeof(AGSEvent<>)).As(typeof(IEvent<>));

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

