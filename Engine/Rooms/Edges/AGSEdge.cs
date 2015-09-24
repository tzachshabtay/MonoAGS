using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSEdge : IEdge
	{
		public AGSEdge()
		{
			OnEdgeCrossed = new AGSEvent<AGSEventArgs> ();
		}

		#region IEdge implementation

		public float Value { get; set; }

		public IEvent<AGSEventArgs> OnEdgeCrossed { get; private set; }

		#endregion
	}
}

