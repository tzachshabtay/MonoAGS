using System;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public class AGSHotspotComponent : AGSComponent, IHotspotComponent
	{
		private Resolver _resolver;
		private IGameEvents _gameEvents;

		public AGSHotspotComponent(IGameEvents gameEvents, Resolver resolver)
		{
			_resolver = resolver;
			_gameEvents = gameEvents;
		}

		public override void Init(IEntity entity)
		{
			base.Init(entity);
			TypedParameter defaults = new TypedParameter (typeof(IInteractions), _gameEvents.DefaultInteractions);
			TypedParameter objParam = new TypedParameter (typeof(IObject), entity as IObject);
			Interactions = _resolver.Container.Resolve<IInteractions>(defaults, objParam);
		}

		public IInteractions Interactions { get; private set; }

		public IPoint WalkPoint { get; set; }

		public string Hotspot { get; set; }
	}
}

