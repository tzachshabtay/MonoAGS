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
        /// The visible area (everything else will be cropped), where (0,0) is the top left corner.
        /// </summary>
        RectangleF CropArea { get; set; }

        /// <summary>
        /// An event which fires whenever the crop area changes.
        /// </summary>
        IEvent<object> OnCropAreaChanged { get; }
    }
}
