using System;
using AGS.API;

namespace AGS.Engine
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

