using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSInteractions : IInteractions
	{
		public AGSInteractions ()
		{
			OnLook = new AGSEvent<ObjectEventArgs> ();
			OnInteract = new AGSEvent<ObjectEventArgs> ();
			OnInventoryInteract = new AGSEvent<InventoryInteractEventArgs> ();
			OnCustomInteract = new AGSEvent<CustomInteractionEventArgs> ();
		}
			
		#region IInteractions implementation

		public IEvent<ObjectEventArgs> OnLook { get; private set; }
		public IEvent<ObjectEventArgs> OnInteract { get; private set; }
		public IEvent<InventoryInteractEventArgs> OnInventoryInteract { get; private set; }
		public IEvent<CustomInteractionEventArgs> OnCustomInteract { get; private set; }

		#endregion
	}
}

