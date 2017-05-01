namespace AGS.API
{
    /// <summary>
    /// Each room has specific events which you can subscribe to and code stuff to happen on those events.
    /// </summary>
    /// <seealso cref="IRoom"/>
    public interface IRoomEvents
	{
        /// <summary>
        /// This event is triggered after leaving the previous room, before the current room is visible.
        /// </summary>
        /// <value>The event.</value>
		IEvent<AGSEventArgs> OnBeforeFadeIn { get; }

        /// <summary>
        /// This event is triggered after leaving the previous room, after the current room is visible.
        /// </summary>
        /// <value>The event.</value>
		IEvent<AGSEventArgs> OnAfterFadeIn { get; }

        /// <summary>
        /// This event is triggered before leaving the current room when it is still visible.
        /// </summary>
        /// <value>The event.</value>
		IEvent<AGSEventArgs> OnBeforeFadeOut { get; }

        /// <summary>
        /// This event is triggered before leaving the current room when it is no longer visible.
        /// </summary>
        /// <value>The event.</value>
		IEvent<AGSEventArgs> OnAfterFadeOut { get; }
	}
}

