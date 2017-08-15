using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public class AGSGameEvents : IGameEvents
	{
		public AGSGameEvents(IEvent onLoad, IEvent onRepeatedlyExecute,
			IBlockingEvent onBeforeRender, IBlockingEvent onScreenResize,
            IEvent onSavedGameLoad, IEvent onRoomChanging, Resolver resolver)
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

		public IEvent OnLoad { get; private set; }

		public IEvent OnRepeatedlyExecute { get; private set; }

		public IBlockingEvent OnBeforeRender { get; private set; }

		public IBlockingEvent OnScreenResize { get; private set; }

		public IEvent OnSavedGameLoad { get; private set; }

		public IInteractions DefaultInteractions { get; private set; }

        public IEvent OnRoomChanging { get; private set; }

		#endregion
	}
}

