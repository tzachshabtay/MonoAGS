namespace AGS.API
{
    /// <summary>
    /// An interface for rendering image on screen. There's a default renderer
    /// which is used for object, which you can override with your own implementation (though
    /// this will require advanced coding knowledge).
    /// </summary>
    public interface IImageRenderer
	{
        /// <summary>
        /// Allows the renderer to return a custom image size that will override the object's image size 
        /// when calculating the object's matrix.
        /// </summary>
        /// <value>The size of the custom image.</value>
        SizeF? CustomImageSize { get; }

        /// <summary>
        /// Allows the renderer to return a custom resolution for the object that will override the object's
        /// resolution when calculating the object's matrix.
        /// </summary>
        /// <value>The custom image resolution factor.</value>
        PointF? CustomImageResolutionFactor { get; }

        /// <summary>
        /// Called before the 'Render' method is called for each existing object, even if it will
        /// not be rendered. 
        /// This is called by the main loop of the game, on each tick and should not be called by the user.
        /// </summary>
        /// <returns>The prepare.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="drawable">Drawable.</param>
        /// <param name="viewport">Viewport.</param>
		void Prepare(IObject obj, IDrawableInfo drawable, IViewport viewport);

        /// <summary>
        /// Renders the specified object on screen.
        /// This is called by the main loop of the game, on each tick and should not be called by the user.
        /// </summary>
        /// <returns>The render.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="viewport">Viewport.</param>
		void Render(IObject obj, IViewport viewport);
	}
}

