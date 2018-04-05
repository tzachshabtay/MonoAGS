using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public class AGSGameEvents : IGameEvents
	{
        public AGSGameEvents(IBlockingEvent onLoad, IEvent<IRepeatedlyExecuteEventArgs> onRepeatedlyExecute,
            IEvent<IRepeatedlyExecuteEventArgs> onRepeatedlyExecuteAlways,
			IBlockingEvent onBeforeRender, IBlockingEvent onScreenResize,
            IBlockingEvent onSavedGameLoad, IBlockingEvent onRoomChanging, IDefaultInteractions defaultInteractions)
		{
			OnLoad = onLoad;
			OnRepeatedlyExecute = onRepeatedlyExecute;
            OnRepeatedlyExecuteAlways = onRepeatedlyExecuteAlways;
			OnBeforeRender = onBeforeRender;
			OnScreenResize = onScreenResize;
			OnSavedGameLoad = onSavedGameLoad;
            OnRoomChanging = onRoomChanging;
            DefaultInteractions = defaultInteractions;
		}

		#region IGameEvents implementation

        public IBlockingEvent OnLoad { get; private set; }

        public IEvent<IRepeatedlyExecuteEventArgs> OnRepeatedlyExecute { get; private set; }

        public IEvent<IRepeatedlyExecuteEventArgs> OnRepeatedlyExecuteAlways { get; private set; }

		public IBlockingEvent OnBeforeRender { get; private set; }

		public IBlockingEvent OnScreenResize { get; private set; }

        public IBlockingEvent OnSavedGameLoad { get; private set; }

		public IDefaultInteractions DefaultInteractions { get; private set; }

        public IBlockingEvent OnRoomChanging { get; private set; }

		#endregion
	}
}

