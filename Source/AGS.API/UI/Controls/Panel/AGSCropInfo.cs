namespace AGS.API
{
    /// <summary>
    /// If the item was completely cropped, gets the direction the item was cropped from.
    /// </summary>
    public enum CropFrom
    {
        /// <summary>
        /// The item was not fully cropped.
        /// </summary>
        None,
        /// <summary>
        /// The item was cropped from the top.
        /// </summary>
        Top,
        /// <summary>
        /// The item was cropped from the bottom.
        /// </summary>
        Bottom,
        /// <summary>
        /// The item was cropped from the left.
        /// </summary>
        Left,
        /// <summary>
        /// The item was cropped from the right.
        /// </summary>
        Right,
    }

    /// <summary>
    /// Information about how to crop entities.
    /// </summary>
    public struct AGSCropInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.AGSCropInfo"/> struct.
        /// </summary>
        /// <param name="boundingBox">Bounding box.</param>
        /// <param name="textureBox">Texture box.</param>
        /// <param name="cropFrom">The direction the item was cropped from, if fully cropped.</param>
        public AGSCropInfo(AGSBoundingBox boundingBox, FourCorners<Vector2> textureBox, CropFrom cropFrom)
        {
            TextureBox = textureBox;
            BoundingBox = boundingBox;
            CropFrom = cropFrom;
        }

        /// <summary>
        /// Gets the texture box ((0,0) - (1,1) is the entire texture, meaning will not be cropped).
        /// </summary>
        /// <value>The texture box.</value>
        public FourCorners<Vector2> TextureBox { get; }

        /// <summary>
        /// Gets the bounding box for rendering the texture in.
        /// </summary>
        /// <value>The bounding box.</value>
        public AGSBoundingBox BoundingBox { get; }

        /// <summary>
        /// If the item was completely cropped, gets the direction the item was cropped from.
        /// </summary>
        /// <value>The direction it was cropped from</value>
        public CropFrom CropFrom { get; }
    }
}
