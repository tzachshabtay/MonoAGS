using System;

namespace AGS.API
{
	public interface IGameEvents
	{
		IEvent<AGSEventArgs> OnLoad { get; }
		IEvent<AGSEventArgs> OnRepeatedlyExecute { get; }

		IInteractions DefaultInteractions { get; }
	}
}

