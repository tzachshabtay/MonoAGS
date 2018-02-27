using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public class AGSGameEvents : IGameEvents
	{
        public AGSGameEvents(IBlockingEvent onLoad, IEvent<IRepeatedlyExecuteEventArgs> onRepeatedlyExecute,
            IEvent<IRepeatedlyExecuteEventArgs> onRepeatedlyExecuteAlways,
			IBlockingEvent onBeforeRender, IBlockingEvent onScreenResize,
            IBlockingEvent onSavedGameLoad, IBlockingEvent onRoomChanging, Resolver resolver)
		{
			OnLoad = onLoad;
			OnRepeatedlyExecute = onRepeatedlyExecute;
            OnRepeatedlyExecuteAlways = onRepeatedlyExecuteAlways;
			OnBeforeRender = onBeforeRender;
			OnScreenResize = onScreenResize;
			OnSavedGameLoad = onSavedGameLoad;
            OnRoomChanging = onRoomChanging;

			TypedParameter nullDefaults = new TypedParameter (typeof(IInteractions), null);
			TypedParameter nullObject = new TypedParameter (typeof(IObject), null);
			DefaultInteractions = resolver.Container.Resolve<IInteractions>(nullDefaults, nullObject);
		}

		#region IGameEvents implementation

        public IBlockingEvent OnLoad { get; private set; }

        public IEvent<IRepeatedlyExecuteEventArgs> OnRepeatedlyExecute { get; private set; }

        public IEvent<IRepeatedlyExecuteEventArgs> OnRepeatedlyExecuteAlways { get; private set; }

		public IBlockingEvent OnBeforeRender { get; private set; }

		public IBlockingEvent OnScreenResize { get; private set; }

        public IBlockingEvent OnSavedGameLoad { get; private set; }

		public IInteractions DefaultInteractions { get; private set; }

        public IBlockingEvent OnRoomChanging { get; private set; }

		#endregion
	}
}

