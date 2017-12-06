namespace AGS.API
{
    /// <summary>
    /// Adds the ability to crop the shown image (only show part of the image).
    /// </summary>
    public interface ICropSelfComponent : IComponent
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ICropSelfComponent"/> crop is enabled.
        /// </summary>
        /// <value><c>true</c> if crop enabled; otherwise, <c>false</c>.</value>
        bool CropEnabled { get; set; }

        /// <summary>
        /// The visible area (everything else will be cropped), where (0,0) is the bottom-left corner.
        /// </summary>
        RectangleF CropArea { get; set; }

        /// <summary>
        /// An event which fires before cropping the area, allows to change the crop configuration.
        /// </summary>
        /// <value>The on before crop.</value>
        IEvent<BeforeCropEventArgs> OnBeforeCrop { get; }

        /// <summary>
        /// Calculates and crops both the texture and the bounding box (used by the engine).
        /// </summary>
        /// <returns>The texture area to render.</returns>
        /// <param name="eventArgs">The event arguments which will be fired by the component before cropping</param>
        /// <param name="spriteWidth">Sprite width.</param>
        /// <param name="spriteHeight">Sprite height.</param>
        /// <param name="width">The bounding box width.</param>
        /// <param name="height">The bounding box height.</param>
        FourCorners<Vector2> GetCropArea(BeforeCropEventArgs eventArgs, float spriteWidth, float spriteHeight, out float width, out float height);
    }
}
