namespace AGS.API
{
    public interface IGameEvents
	{
		IEvent<AGSEventArgs> OnLoad { get; }
		IEvent<AGSEventArgs> OnRepeatedlyExecute { get; }
		IEvent<AGSEventArgs> OnSavedGameLoad { get; }

		IInteractions DefaultInteractions { get; }
	}
}

