namespace AGS.API
{
    /// <summary>
    /// Represents an image that can be rendered on screen (usually as part of an animation).
    /// </summary>
    public interface IImage
	{
        /// <summary>
        /// Gets the original bitmap that this image is taken from.
        /// </summary>
        /// <value>
        /// The original bitmap.
        /// </value>
        IBitmap OriginalBitmap { get; }

        /// <summary>
        /// Gets the width of the image (in pixels).
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        float Width { get; }

        /// <summary>
        /// Gets the height of the image (in pixels).
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        float Height { get; }

        /// <summary>
        /// Gets the unique ID that represents that image. It will usually be the file path (unless
        /// the image is part of a sprite sheet, in that case the file path will not be a unique identifier).
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        string ID { get; }

        /// <summary>
        /// If the image is part of a sprite sheet, this will return the sprite sheet data.
        /// Otherwise, this will return null.
        /// </summary>
        /// <value>
        /// The sprite sheet.
        /// </value>
        ISpriteSheet SpriteSheet { get; }

        /// <summary>
        /// Gets the configuration with which the image was loaded.
        /// </summary>
        /// <value>
        /// The load configuration.
        /// </value>
        ILoadImageConfig LoadConfig { get; }
	}
}

