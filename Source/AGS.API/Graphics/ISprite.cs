namespace AGS.API
{
    /// <summary>
    /// A sprite is an image which can be manipulated (scaled, rotated or translated in space).
    /// </summary>
    public interface ISprite : IHasImage, ITranslate, IScale, IPixelPerfectCollidable, IRotate, 
        IHasModelMatrix
	{
        /// <summary>
        /// Clone this sprite. This is a deep clone, meaning the generated sprite will be detached
        /// from the existing sprite.
        /// </summary>
        /// <returns>The cloned sprite.</returns>
        ISprite Clone();
	}

    /// <summary>
    /// An interface representing an entity which has a model matrix and can be rendered on screen.
    /// </summary>
    public interface IHasModelMatrix : IScale, IRotate, ITranslate, IHasImage
    {
    }
}

