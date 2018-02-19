using System;
using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// A resource pack is an abstract representation of a storage space for retrieving resources.
    /// There can be a resource pack that's based on the file system, a resource pack which retrieves embedded resource in the game file itself,
    /// a resource pack that downloads resources from the web, a resource pack based on a zip file, etc.
    /// <seealso cref="IResourceLoader"/>
    /// </summary>
    public interface IResourcePack
    {
        /// <summary>
        /// Gets the priority of the pack which is used by the <see cref="IResourceLoader"/> for determining which resource pack is queried first (higher priority gets precedence).
        /// </summary>
        /// <value>The priority.</value>
        int Priority { get; }

        /// <summary>
        /// Loads a resource from a embedded/file path.
        /// </summary>
        /// <returns>The resource.</returns>
        /// <param name="path">Path.</param>
        IResource LoadResource(string path);

        /// <summary>
        /// Loads all the the resource available in the specified embedded/file system folder.
        /// </summary>
        /// <returns>The resources.</returns>
        /// <param name="folder">Folder.</param>
        List<IResource> LoadResources(string folder);

        /// <summary>
        /// Finds a matching resource pack specific path for the specified global resource path.
        /// </summary>
        /// <returns>The resource pack path if found a matching resource, otherwise null.</returns>
        /// <param name="path">Path.</param>
        string ResolvePath(string path);
    }
}
