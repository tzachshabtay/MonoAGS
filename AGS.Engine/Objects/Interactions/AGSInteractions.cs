using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSInteractions : IInteractions
	{
		public AGSInteractions (IInteractions defaultInteractions, IObject obj, IPlayer player)
		{
			OnLook = new AGSInteractionEvent<ObjectEventArgs>(defaultInteractions == null ? null : 
				defaultInteractions.OnLook, true, obj, player);
			OnInteract = new AGSInteractionEvent<ObjectEventArgs> (defaultInteractions == null ? null : 
				defaultInteractions.OnInteract, false, obj, player);
			OnInventoryInteract = new AGSInteractionEvent<InventoryInteractEventArgs> (defaultInteractions == null ? null : 
				defaultInteractions.OnInventoryInteract, false, obj, player);
			OnCustomInteract = new AGSInteractionEvent<CustomInteractionEventArgs> (defaultInteractions == null ? null : 
				defaultInteractions.OnCustomInteract, false, obj, player);
		}
			
		#region IInteractions implementation

		public IEvent<ObjectEventArgs> OnLook { get; private set; }
		public IEvent<ObjectEventArgs> OnInteract { get; private set; }
		public IEvent<InventoryInteractEventArgs> OnInventoryInteract { get; private set; }
		public IEvent<CustomInteractionEventArgs> OnCustomInteract { get; private set; }

		#endregion
	}
}

