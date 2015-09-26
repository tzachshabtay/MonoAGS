using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSInteractions : IInteractions
	{
		public AGSInteractions (IInteractions defaultInteractions)
		{
			OnLook = new AGSInteractionEvent<ObjectEventArgs>(defaultInteractions == null ? null : defaultInteractions.OnLook);
			OnInteract = new AGSInteractionEvent<ObjectEventArgs> (defaultInteractions == null ? null : defaultInteractions.OnInteract);
			OnInventoryInteract = new AGSInteractionEvent<InventoryInteractEventArgs> (defaultInteractions == null ? null : defaultInteractions.OnInventoryInteract);
			OnCustomInteract = new AGSInteractionEvent<CustomInteractionEventArgs> (defaultInteractions == null ? null : defaultInteractions.OnCustomInteract);
		}
			
		#region IInteractions implementation

		public IEvent<ObjectEventArgs> OnLook { get; private set; }
		public IEvent<ObjectEventArgs> OnInteract { get; private set; }
		public IEvent<InventoryInteractEventArgs> OnInventoryInteract { get; private set; }
		public IEvent<CustomInteractionEventArgs> OnCustomInteract { get; private set; }

		#endregion
	}
}

