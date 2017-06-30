namespace AGS.API
{
    /// <summary>
    /// Represents the render loop of the game. All drawing to the screen is performed from the render loop.
    /// </summary>
    public interface IRendererLoop
	{
        /// <summary>
        /// Calls the next tick, which triggers the next render.
        /// This is called from within the engine.
        /// </summary>
        /// <returns>True if a render is performed (can return false if we're in the midst of a room transition)</returns>
        bool Tick();

        /// <summary>
        /// An event that fires on each tick before rendering the display list.
        /// This allows viewing (and even modifying) the display list prior to rendering it.
        /// </summary>
        /// <returns>The event.</returns>
        IEvent<DisplayListEventArgs> OnBeforeRenderingDisplayList { get; }
	}
}

