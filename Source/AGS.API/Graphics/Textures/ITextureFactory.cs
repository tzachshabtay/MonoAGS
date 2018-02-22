namespace AGS.API
{
    /// <summary>
    /// A factory for creating textures.
    /// </summary>
    public interface ITextureFactory
    {
        /// <summary>
        /// Creates the texture from the identifier (or returns an empty texture if the id is null).
        /// </summary>
        /// <returns>The texture.</returns>
        /// <param name="id">The texture id (path in the file system, for example).</param>
        ITexture CreateTexture(string id);
    }
}
