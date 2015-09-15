using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSGameEvents : IGameEvents
	{
		public AGSGameEvents(IEvent<EventArgs> onLoad, IEvent<EventArgs> onRepeatedlyExecute)
		{
			OnLoad = onLoad;
			OnRepeatedlyExecute = onRepeatedlyExecute;
		}

		#region IGameEvents implementation

		public IEvent<EventArgs> OnLoad { get; private set; }

		public IEvent<EventArgs> OnRepeatedlyExecute { get; private set; }

		#endregion
	}
}

