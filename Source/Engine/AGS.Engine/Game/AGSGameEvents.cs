using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public class AGSGameEvents : IGameEvents
	{
		public AGSGameEvents(IEvent<object> onLoad, IEvent<object> onRepeatedlyExecute,
			IBlockingEvent<object> onBeforeRender, IBlockingEvent<object> onScreenResize,
            IEvent<object> onSavedGameLoad, IEvent<object> onRoomChanging, Resolver resolver)
		{
			OnLoad = onLoad;
			OnRepeatedlyExecute = onRepeatedlyExecute;
			OnBeforeRender = onBeforeRender;
			OnScreenResize = onScreenResize;
			OnSavedGameLoad = onSavedGameLoad;
            OnRoomChanging = onRoomChanging;

			TypedParameter nullDefaults = new TypedParameter (typeof(IInteractions), null);
			TypedParameter nullObject = new TypedParameter (typeof(IObject), null);
			DefaultInteractions = resolver.Container.Resolve<IInteractions>(nullDefaults, nullObject);
		}

		#region IGameEvents implementation

		public IEvent<object> OnLoad { get; private set; }

		public IEvent<object> OnRepeatedlyExecute { get; private set; }

		public IBlockingEvent<object> OnBeforeRender { get; private set; }

		public IBlockingEvent<object> OnScreenResize { get; private set; }

		public IEvent<object> OnSavedGameLoad { get; private set; }

		public IInteractions DefaultInteractions { get; private set; }

        public IEvent<object> OnRoomChanging { get; private set; }

		#endregion
	}
}

