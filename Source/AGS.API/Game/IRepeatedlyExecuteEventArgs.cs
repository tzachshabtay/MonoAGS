namespace AGS.API
{
    /// <summary>
    /// Event arguments for the <see cref="IGameEvents.OnRepeatedlyExecute"/> event.
    /// </summary>
    public interface IRepeatedlyExecuteEventArgs
    {
        /// <summary>
        /// Gets the delta time in seconds from the last tick (the last update event).
        /// You can use it to achieve framerate independence for custom movements. See here: https://www.scirra.com/tutorials/67/delta-time-and-framerate-independence
        /// </summary>
        /// <value>The delta time.</value>
        double DeltaTime { get; }
    }
}
