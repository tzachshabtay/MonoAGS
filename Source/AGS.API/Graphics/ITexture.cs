namespace AGS.API
{
    public interface ITexture
    {
        /// <summary>
        /// Gets the texture identifier.
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Gets the configuration of the texture (scaling filter, should the texture be tiled or not).
        /// </summary>
        ITextureConfig Config { get; set; }
    }
}
