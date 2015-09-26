using System;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public class AGSGameEvents : IGameEvents
	{
		public AGSGameEvents(IEvent<AGSEventArgs> onLoad, IEvent<AGSEventArgs> onRepeatedlyExecute,
			Resolver resolver)
		{
			OnLoad = onLoad;
			OnRepeatedlyExecute = onRepeatedlyExecute;

			TypedParameter nullDefaults = new TypedParameter (typeof(IInteractions), null);
			DefaultInteractions = resolver.Container.Resolve<IInteractions>(nullDefaults);
		}

		#region IGameEvents implementation

		public IEvent<AGSEventArgs> OnLoad { get; private set; }

		public IEvent<AGSEventArgs> OnRepeatedlyExecute { get; private set; }

		public IInteractions DefaultInteractions { get; private set; }

		#endregion
	}
}

