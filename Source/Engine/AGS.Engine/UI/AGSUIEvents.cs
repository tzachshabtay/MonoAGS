using AGS.API;

namespace AGS.Engine
{
	public class AGSUIEvents : AGSComponent, IUIEvents
	{
        private readonly UIEventsAggregator _aggregator;
        private IEntity _entity;

        public AGSUIEvents(UIEventsAggregator aggregator)
		{
            _aggregator = aggregator;

			MouseEnter = new AGSEvent<MousePositionEventArgs> ();
			MouseLeave = new AGSEvent<MousePositionEventArgs> ();
			MouseMove = new AGSEvent<MousePositionEventArgs> ();
			MouseClicked = new AGSEvent<MouseButtonEventArgs> ();
            MouseDoubleClicked = new AGSEvent<MouseButtonEventArgs>();
            MouseDown = new AGSEvent<MouseButtonEventArgs> ();
			MouseUp = new AGSEvent<MouseButtonEventArgs> ();
            LostFocus = new AGSEvent<MouseButtonEventArgs>();
        }

		public override void Init(IEntity entity)
		{
			base.Init(entity);
            _entity = entity;
			var enabled = entity.GetComponent<IEnabledComponent>();
			var visible = entity.GetComponent<IVisibleComponent>();
            _aggregator.Subscribe(entity, mouseIn => IsMouseIn = mouseIn, this, enabled, visible);
		}

        public override void Dispose()
        {
            base.Dispose();
            _aggregator.Unsubscribe(_entity);
        }

		public IEvent<MousePositionEventArgs> MouseEnter { get; private set; }

		public IEvent<MousePositionEventArgs> MouseLeave { get; private set; }

		public IEvent<MousePositionEventArgs> MouseMove { get; private set; }

		public IEvent<MouseButtonEventArgs> MouseClicked { get; private set; }

        public IEvent<MouseButtonEventArgs> MouseDoubleClicked { get; private set; }

        public IEvent<MouseButtonEventArgs> MouseDown { get; private set; }

		public IEvent<MouseButtonEventArgs> MouseUp { get; private set; }

        public IEvent<MouseButtonEventArgs> LostFocus { get; private set; }

        public bool IsMouseIn { get; private set; }
	}
}

