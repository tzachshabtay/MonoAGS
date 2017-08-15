namespace AGS.API
{
    /// <summary>
    /// Bounding box type.
    /// </summary>
    public enum BoundingBoxType 
    {
        /// <summary>
        /// Bounding box which is used for rendering an entity. 
        /// This will be in the object's resolution.
        /// </summary>
        Render,
        /// <summary>
        /// Bounding box which is used for collision tests.
        /// This will be in the game's resolution.
        /// </summary>
        HitTest,
    }

    /// <summary>
    /// Event arguments for the "before crop" event.
    /// </summary>
    public struct BeforeCropEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.BeforeCropEventArgs"/> struct.
        /// </summary>
        /// <param name="boundingBox">Bounding box.</param>
        /// <param name="boundingBoxType">Bounding box type.</param>
        public BeforeCropEventArgs(AGSBoundingBox boundingBox, BoundingBoxType boundingBoxType)
        {
            BoundingBox = boundingBox;
            BoundingBoxType = boundingBoxType;
        }

        /// <summary>
        /// Gets the bounding box to be cropped.
        /// </summary>
        /// <value>The bounding box.</value>
        public AGSBoundingBox BoundingBox { get; private set; }

        /// <summary>
        /// Gets the type of the bounding box to be cropped.
        /// </summary>
        /// <value>The type of the bounding box.</value>
        public BoundingBoxType BoundingBoxType { get; private set; }
    }
}
