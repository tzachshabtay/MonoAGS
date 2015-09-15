using System;

namespace AGS.API
{
	public interface IGameEvents
	{
		IEvent<EventArgs> OnLoad { get; }
		IEvent<EventArgs> OnRepeatedlyExecute { get; }
	}
}

