using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// The resource loader is the go-to place for retrieving resources.
    /// It does so by querying various resources packs for the resource you require until it is found.
    /// Each resource pack has a priority which determines the order of precedence in which the packs are queried (higher priority gets precedence).
    /// 
    /// There are currently 2 built-in resource packs in the engine, which allow for 2 ways you can have resources in your game: 
    /// They can be embedded in the project or loaded from the file system.
    /// This interface allows for loading resource either from the embedded project files or from the file system.
    /// 
    /// The advantage of having the resources embedded is that you can rest assured the resources will be distributed with your game and cannot be touched from outside, 
    /// which is why it's the recommended method. Loading from file system might be useful if you need the resources to 
    /// be loaded dynamically, for example you might want to download resources from the internet, or if you give the 
    /// user the option to choose her/his avatar.
    /// To embed resources in your game project, first add the resource files to the "Assets" folder in your shared game 
    /// project(it doesn't have to be in the root "Assets" folder, you can have any structure you want in there). 
    /// Then, in the solution explorer, right click the "Assets" folder and "Add Existing" (there are options for 
    /// adding files or complete folders, depending on what you want to do) and add those resources to the project. 
    /// You should then see those resources in the tree. Lastly, right click those resources and select 
    /// "Embedded Resource" as your "Build Option".
    /// Note that even the resource is embedded, it's only embedded when compiling the game, so you cannot delete the 
    /// file before deploying your game, and if you replace the file, it will be automatically replaced in the game 
    /// on your next run.
    /// 
    /// The path used by the loading methods has to be structured as so: if the resource is to be loaded from a file 
    /// in the file system, then put the absolute path of the file. If the resource is embedded then put the relative 
    /// path of the file (when the current folder is the "Assets" folder). So, for example, if you have an audio file called "trumpet.ogg" sitting under a 
    /// "Sounds" folder in "Assets", your path would be "Sounds/trumpet.ogg". Note that this would 
    /// work even if the file is sitting in that folder but not embedded. This is because (assuming the resource loader is configured
    /// with both an embedded resource pack and a file system resource pack) `ResourceLoader` will search for both an embedded resource and for 
    /// a file from the file system. The order of the search depends on the configured priority for each resource pack.
    /// </summary>
    public interface IResourceLoader : IResourcePack
	{
        /// <summary>
        /// Allows adding/removing resource packs from the resource loader.
        /// </summary>
        /// <value>The resource packs.</value>
        IAGSBindingList<ResourcePack> ResourcePacks { get; }

        /// <summary>
        /// Loads a list of resources from a list of embedded/file paths.
        /// </summary>
        /// <returns>The resources.</returns>
        /// <param name="paths">Paths.</param>
        List<IResource> LoadResourcesFromPaths(params string[] paths);
	}

    /// <summary>
    /// A resource pack to be registered with the resource loader. It carries both the resource pack implementation,
    /// and a priority which used to decide which resource pack takes precedence in the resource loader (higher is better).
    /// </summary>
    public struct ResourcePack
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.ResourcePack"/> struct.
        /// </summary>
        /// <param name="pack">Pack.</param>
        /// <param name="priority">Priority.</param>
        public ResourcePack(IResourcePack pack, int priority)
        {
            Pack = pack;
            Priority = priority;
        }

        /// <summary>
        /// Gets the resource pack.
        /// </summary>
        /// <value>The resource pack.</value>
        public IResourcePack Pack { get; private set; }

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority { get; private set; }
    }
}