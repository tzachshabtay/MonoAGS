using System;
namespace AGS.API
{
    /// <summary>
    /// A single cache which stores all of the textures in a single place.
    /// </summary>
    public interface ITextureCache
    {
        /// <summary>
        /// Gets a texture (or creates a texture using the factory function if the texture doesn't exist).
        /// </summary>
        /// <returns>The texture.</returns>
        /// <param name="id">The texture id (path on the file system, for example).</param>
        /// <param name="factory">A function to create the texture if the texture does not exist already.</param>
        ITexture GetTexture(string id, Func<string, ITexture> factory);

        /// <summary>
        /// Tries to get the texture.
        /// </summary>
        /// <returns><c>true</c>, if the texture exists, <c>false</c> otherwise.</returns>
        /// <param name="id">The texture id (path on the file system, for example).</param>
        /// <param name="texture">The returned texture (or null if it doesn't exist).</param>
        bool TryGetTexture(string id, out ITexture texture);

        /// <summary>
        /// Removes the texture from the cache.
        /// </summary>
        /// <param name="id">The texture id (path on the file system, for example).</param>
        void RemoveTexture(string id);
    }
}
