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
        /// The information on the last crop.
        /// </summary>
        /// <value>The last crop.</value>
        AGSCropInfo LastCrop { get; }

        /// <summary>
        /// An event which fires before cropping the area, allows to change the crop configuration.
        /// </summary>
        /// <value>The on before crop.</value>
        IBlockingEvent<BeforeCropEventArgs> OnBeforeCrop { get; }

        /// <summary>
        /// Calculates and crops both the texture and the bounding box (used by the engine).
        /// </summary>
        /// <returns>The crop info.</returns>
        /// <param name="box">The bounding box to crop.</param>
        /// <param name="boundingBoxType">Which type of bounding box is to be cropped.</param>
        /// <param name="adjustedScale">Adjusted scale.</param>
        AGSCropInfo Crop(ref AGSBoundingBox box, BoundingBoxType boundingBoxType, PointF adjustedScale);

        /// <summary>
        /// Is the entity guaranteed to be fully cropped?
        /// In specific scenario we can know for certain that the entity will be fully cropped from the scene,
        /// which can be used for optimizations in the engine.
        /// For example: if the entity is part of a vertical stack layout and the previous item was cropped from
        /// the top we know that this item will also be fully cropped.
        /// </summary>
        /// <returns><c>true</c>, if guaranteed to fully crop, <c>false</c> otherwise.</returns>
        bool IsGuaranteedToFullyCrop();

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ICropSelfComponent"/> is never guaranteed to
        /// fully crop (so calling <see cref="IsGuaranteedToFullyCrop"/> will always return true, ensuring bounding box calculations
        /// are always performed on this entity (even if it's outside a scrolling panel).
        /// </summary>
        /// <value><c>true</c> if never guaranteed to fully crop; otherwise, <c>false</c>.</value>
        bool NeverGuaranteedToFullyCrop { get; set; }
    }
}