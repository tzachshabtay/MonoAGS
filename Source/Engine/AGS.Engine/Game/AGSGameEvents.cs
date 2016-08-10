using System;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public class AGSGameEvents : IGameEvents
	{
		public AGSGameEvents(IEvent<AGSEventArgs> onLoad, IEvent<AGSEventArgs> onRepeatedlyExecute,
			IBlockingEvent<AGSEventArgs> onBeforeRender, IBlockingEvent<AGSEventArgs> onScreenResize,
			IEvent<AGSEventArgs> onSavedGameLoad, Resolver resolver)
		{
			OnLoad = onLoad;
			OnRepeatedlyExecute = onRepeatedlyExecute;
			OnBeforeRender = onBeforeRender;
			OnScreenResize = onScreenResize;
			OnSavedGameLoad = onSavedGameLoad;

			TypedParameter nullDefaults = new TypedParameter (typeof(IInteractions), null);
			TypedParameter nullObject = new TypedParameter (typeof(IObject), null);
			DefaultInteractions = resolver.Container.Resolve<IInteractions>(nullDefaults, nullObject);
		}

		#region IGameEvents implementation

		public IEvent<AGSEventArgs> OnLoad { get; private set; }

		public IEvent<AGSEventArgs> OnRepeatedlyExecute { get; private set; }

		public IBlockingEvent<AGSEventArgs> OnBeforeRender { get; private set; }

		public IBlockingEvent<AGSEventArgs> OnScreenResize { get; private set; }

		public IEvent<AGSEventArgs> OnSavedGameLoad { get; private set; }

		public IInteractions DefaultInteractions { get; private set; }

		#endregion
	}
}

