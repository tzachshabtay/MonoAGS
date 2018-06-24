using AGS.API;

namespace AGS.Engine
{
	public class AGSUIEvents : AGSComponent, IUIEvents
	{
        private readonly UIEventsAggregator _aggregator;

        public AGSUIEvents(UIEventsAggregator aggregator)
		{
            _aggregator = aggregator;

			MouseEnter = new AGSEvent<MousePositionEventArgs> ();
			MouseLeave = new AGSEvent<MousePositionEventArgs> ();
			MouseMove = new AGSEvent<MousePositionEventArgs> ();
            MouseClicked = new AGSEvent<MouseClickEventArgs> ();
            MouseDoubleClicked = new AGSEvent<MouseClickEventArgs>();
            MouseDown = new AGSEvent<MouseButtonEventArgs> ();
			MouseUp = new AGSEvent<MouseButtonEventArgs> ();
            LostFocus = new AGSEvent<MouseButtonEventArgs>();
        }

		public override void Init()
		{
			base.Init();
			var enabled = Entity.GetComponent<IEnabledComponent>();
            var visible = Entity.GetComponent<IVisibleComponent>();
            _aggregator.Subscribe(Entity, mouseIn => IsMouseIn = mouseIn, this, enabled, visible);
		}

        public override void Dispose()
        {
            _aggregator.Unsubscribe(Entity);
            MouseEnter?.Dispose();
            MouseLeave?.Dispose();
            MouseMove?.Dispose();
            MouseClicked?.Dispose();
            MouseDoubleClicked?.Dispose();
            MouseDown?.Dispose();
            MouseUp?.Dispose();
            LostFocus?.Dispose();
            base.Dispose();
        }

		public IEvent<MousePositionEventArgs> MouseEnter { get; private set; }

		public IEvent<MousePositionEventArgs> MouseLeave { get; private set; }

		public IEvent<MousePositionEventArgs> MouseMove { get; private set; }

		public IEvent<MouseClickEventArgs> MouseClicked { get; private set; }

        public IEvent<MouseClickEventArgs> MouseDoubleClicked { get; private set; }

        public IEvent<MouseButtonEventArgs> MouseDown { get; private set; }

		public IEvent<MouseButtonEventArgs> MouseUp { get; private set; }

        public IEvent<MouseButtonEventArgs> LostFocus { get; private set; }

        public bool IsMouseIn { get; private set; }
	}
}
