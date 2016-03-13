namespace AGS.API
{
    public interface IRoomEvents
	{
		IEvent<AGSEventArgs> OnBeforeFadeIn { get; }

		IEvent<AGSEventArgs> OnAfterFadeIn { get; }

		IEvent<AGSEventArgs> OnBeforeFadeOut { get; }

		IEvent<AGSEventArgs> OnAfterFadeOut { get; }
	}
}

