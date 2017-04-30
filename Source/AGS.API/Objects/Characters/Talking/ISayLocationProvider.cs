namespace AGS.API
{
    /// <summary>
    /// This is called every time a character speaks to determine the location on the screen on which
    /// the text (and the portrait, if portrait rendering is desired) will be rendered. 
    /// </summary>
    public interface ISayLocationProvider
	{
        /// <summary>
        /// Gets the location to render the text and possibly portrait.
        /// </summary>
        /// <returns>The location.</returns>
        /// <param name="text">Text.</param>
        /// <param name="config">Speech configuration, some of it might affect the returned location (like text alignment and padding).</param>
        ISayLocation GetLocation(string text, ISayConfig config);
	}
}

