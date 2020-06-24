using AGS.API;

namespace AGS.Engine
{
	public class AGSGameEvents : IGameEvents
	{
        public AGSGameEvents(IBlockingEvent onLoad, IBlockingEvent<IRepeatedlyExecuteEventArgs> onRepeatedlyExecute,
            IBlockingEvent<IRepeatedlyExecuteEventArgs> onRepeatedlyExecuteAlways,
			IBlockingEvent onBeforeRender, IBlockingEvent onScreenResize,
            IBlockingEvent<IGame> onSavedGameLoad, IBlockingEvent onRoomChanging, IDefaultInteractions defaultInteractions)
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

        public IBlockingEvent<IRepeatedlyExecuteEventArgs> OnRepeatedlyExecute { get; private set; }

        public IBlockingEvent<IRepeatedlyExecuteEventArgs> OnRepeatedlyExecuteAlways { get; private set; }

		public IBlockingEvent OnBeforeRender { get; private set; }

		public IBlockingEvent OnScreenResize { get; private set; }

        public IBlockingEvent<IGame> OnSavedGameLoad { get; private set; }

		public IDefaultInteractions DefaultInteractions { get; private set; }

        public IBlockingEvent OnRoomChanging { get; private set; }

		#endregion
	}
}