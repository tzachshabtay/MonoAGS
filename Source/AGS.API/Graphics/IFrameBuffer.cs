namespace AGS.API
{
    /// <summary>
    /// Allows rendering into a texture instead of rendering to screen
    /// </summary>
	public interface IFrameBuffer
	{
        /// <summary>
        /// Gets the texture that was rendered (will be valid after End() was called).
        /// </summary>
        /// <value>The texture.</value>
        ITexture Texture { get; }

        /// <summary>
        /// Signals that all following renders will be performed into the buffer instead of into the screen.
        /// 'Begin' must be called from the rendering thread.
        /// Note: for room transition implementations, the frame buffer is already rendered with the room data, 
        /// prepared to be used, so calling Begin + End is not needed.
        /// <returns>Returns true if the frame buffer was correctly initialized, false if some error has occured.</returns>
        /// </summary>
        bool Begin();

        /// <summary>
        /// Signals that all following renders will no longer be performed into the buffer, 
        /// they will return to render into the screen (as usual).
        /// 'End' must be called from the rendering thread (and after 'Begin' or it has no value).
        /// Note: for room transition implementations, the frame buffer is already rendered with the room data, 
        /// prepared to be used, so calling Begin + End is not needed.
        /// </summary>
        void End();
	}
}

