using System;

namespace API
{
	public interface IEdge
	{
		float Value { get; set; }
		IEvent<EventArgs> OnEdgeCrossed { get; }
	}
}

