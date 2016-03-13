namespace AGS.API
{
    public interface IInteractions
	{
		IEvent<ObjectEventArgs> OnLook { get; }
		IEvent<ObjectEventArgs> OnInteract { get; }
		IEvent<InventoryInteractEventArgs> OnInventoryInteract { get; }
		IEvent<CustomInteractionEventArgs> OnCustomInteract { get; }
	}
}

