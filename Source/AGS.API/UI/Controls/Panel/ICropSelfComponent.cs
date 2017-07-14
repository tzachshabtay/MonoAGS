namespace AGS.API
{
    /// <summary>
    /// Adds the ability to crop the shown image (only show part of the image).
    /// </summary>
    public interface ICropSelfComponent : IComponent
    {
        /// <summary>
        /// The visible area (everything else will be cropped).
        /// </summary>
        RectangleF CropArea { get; set; }

        /// <summary>
        /// An event which fires whenever the crop area changes.
        /// </summary>
        IEvent<object> OnCropAreaChanged { get; }
    }
}
