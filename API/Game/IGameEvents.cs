using System;

namespace API
{
	public interface IGameEvents
	{
		IEvent<EventArgs> OnLoad { get; }
		IEvent<EventArgs> OnRepeatedlyExecute { get; }
	}
}

