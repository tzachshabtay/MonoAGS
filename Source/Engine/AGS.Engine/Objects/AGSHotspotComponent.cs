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

		public override void Init()
		{
			base.Init();
			TypedParameter defaults = new TypedParameter (typeof(IInteractions), _gameEvents.DefaultInteractions);
            TypedParameter objParam = new TypedParameter (typeof(IObject), Entity as IObject);
			Interactions = _resolver.Container.Resolve<IInteractions>(defaults, objParam);
            DisplayHotspot = true;
		}

        public override void Dispose()
        {
            base.Dispose();
            Interactions = null;
        }

        [Property(Browsable = false)]
		public IInteractions Interactions { get; private set; }

		public PointF? WalkPoint { get; set; }

		public bool DisplayHotspot { get; set; }
	}
}

