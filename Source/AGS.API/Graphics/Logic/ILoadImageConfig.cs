namespace AGS.API
{
    /// <summary>
    /// Configuration to be used when loading images.
    /// </summary>
    public interface ILoadImageConfig
	{
		/// <summary>
		/// For loading non 32-bit images (i.e with no alpha), you can select
		/// any color on the image to act as the transparent color. 
		/// (0,0) for selecting the color on the top-left pixel of the image.
		/// </summary>
		/// <value>The transparent color sample point.</value>
		Point? TransparentColorSamplePoint { get; }

        /// <summary>
        /// Gets the configuration of the texture (scaling filter, should the texture be tiled or not).
        /// </summary>
        ITextureConfig TextureConfig { get; }
	}
}

