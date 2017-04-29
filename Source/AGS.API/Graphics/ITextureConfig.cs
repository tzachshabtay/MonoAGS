namespace AGS.API
{
    /// <summary>
    /// The filter to be used when scaling up the texture.
    /// </summary>
    public enum ScaleUpFilters
    {
        /// <summary>
        /// Uses a weighted average of the closest pixels.
        /// Gives a smooth look (recommended for non pixel art games).
        /// </summary>
        Linear,
        /// <summary>
        /// Uses the nearest neighbor (in Manhattan Distance) as the neighbor to be used.
        /// Gives a pixelated look (recommended for pixel art games).
        /// </summary>
        Nearest,
    }

    /// <summary>
    /// The filter to be used when scaling down the texture.
    /// </summary>
    public enum ScaleDownFilters
    {
        /// <summary>
        /// Uses a weighted average of the closest pixels.
        /// Gives a smooth look (recommended for non pixel art games).
        /// </summary>
        Linear,
        /// <summary>
        /// Uses a weighted average to choose the mipmap, then a weighted average to choose the pixel.
        /// </summary>
        LinearMipmapLinear,
        /// <summary>
        /// Uses a weighted average to choose the mipmap, then the nearest neighbor to choose the pixel.
        /// </summary>
        LinearMipmapNearest,
        /// <summary>
        /// Uses the nearest neighbor (in Manhattan Distance) as the neighbor to be used.
        /// Gives a pixelated look (recommended for pixel art games).
        /// </summary>
        Nearest,
        /// <summary>
        /// Uses the closest mipmap, then a weighted average to choose the pixel.
        /// </summary>
        NearestMipmapLinear,
        /// <summary>
        /// Uses the closest mipmap, then the closest neighbor to choose the pixel.
        /// </summary>
        NearestMipmapNearest,
    }

    /// <summary>
    /// How a texture is wrapped to fill an image.
    /// </summary>
    public enum TextureWrap
    {
        /// <summary>
        /// The texture will be stretched if needed to match the image.
        /// </summary>
        Clamp,
        /// <summary>
        /// The texture will be tiled in a forwards-backwards-forwards-.. to match the image.
        /// </summary>
        MirroredRepeat,
        /// <summary>
        /// The texture will be tiled by repeating it as needed to match the image.
        /// </summary>
        Repeat,
    }

    /// <summary>
    /// Various configurations for a texture.
    /// </summary>
    public interface ITextureConfig
    {
        /// <summary>
        /// Which filters will be used to scale up the texture if needed?
        /// </summary>
        ScaleUpFilters ScaleUpFilter { get; }
        /// <summary>
        /// Which filters will be used to scale down the texture if needed?
        /// </summary>
        ScaleDownFilters ScaleDownFilter { get; }
        /// <summary>
        /// How would the texture be wrapped to match the image on its horizontal axis?
        /// </summary>
        TextureWrap WrapX { get; }
        /// <summary>
        /// How would the texture be wrapped to match the image on its vertical axis?
        /// </summary>
        TextureWrap WrapY { get; }
    }
}
