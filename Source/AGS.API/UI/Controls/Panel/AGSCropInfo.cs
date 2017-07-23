namespace AGS.API
{
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
        public AGSCropInfo(AGSBoundingBox boundingBox, FourCorners<Vector2> textureBox)
        {
            TextureBox = textureBox;
            BoundingBox = boundingBox;
        }

        /// <summary>
        /// Gets the texture box ((0,0) - (1,1) is the entire texture, meaning will not be cropped).
        /// </summary>
        /// <value>The texture box.</value>
        public FourCorners<Vector2> TextureBox { get; private set; }

        /// <summary>
        /// Gets the bounding box for rendering the texture in.
        /// </summary>
        /// <value>The bounding box.</value>
        public AGSBoundingBox BoundingBox { get; private set; }
    }
}
