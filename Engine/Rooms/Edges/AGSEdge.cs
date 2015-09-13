using System;
using API;

namespace Engine
{
	public class AGSEdge : IEdge
	{
		public AGSEdge()
		{
			OnEdgeCrossed = new AGSEvent<EventArgs> ();
		}

		#region IEdge implementation

		public float Value { get; set; }

		public IEvent<EventArgs> OnEdgeCrossed { get; private set; }

		#endregion
	}
}

