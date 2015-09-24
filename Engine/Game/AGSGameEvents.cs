using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSGameEvents : IGameEvents
	{
		public AGSGameEvents(IEvent<AGSEventArgs> onLoad, IEvent<AGSEventArgs> onRepeatedlyExecute)
		{
			OnLoad = onLoad;
			OnRepeatedlyExecute = onRepeatedlyExecute;
		}

		#region IGameEvents implementation

		public IEvent<AGSEventArgs> OnLoad { get; private set; }

		public IEvent<AGSEventArgs> OnRepeatedlyExecute { get; private set; }

		#endregion
	}
}

