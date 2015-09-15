using System;

namespace AGS.API
{
	public interface IEdge
	{
		float Value { get; set; }
		IEvent<EventArgs> OnEdgeCrossed { get; }
	}
}

