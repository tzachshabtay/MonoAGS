using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// There are 2 ways you can have resources in your game: 
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
    /// path of the file (when the current folder is the folder where the executable sits, which is 2 folders above 
    /// the "Assets" folder). So, for example, if you have an audio file called "trumpet.ogg" sitting under a 
    /// "Sounds" folder in "Assets", your path would be "../../Assets/Sounds/trumpet.ogg". Note that this would 
    /// work even if the file is sitting in that folder but not embedded. This is because `ResourceLoader` first 
    /// searches for an embedded resource with that path, but if one is not found, it looks for the file in the 
    /// file system.
    /// </summary>
    public interface IResourceLoader
	{
        /// <summary>
        /// Loads a resource from a embedded/file path.
        /// </summary>
        /// <returns>The resource.</returns>
        /// <param name="path">Path.</param>
		IResource LoadResource(string path);

        /// <summary>
        /// Loads a list of resources from a list of embedded/file paths.
        /// </summary>
        /// <returns>The resources.</returns>
        /// <param name="paths">Paths.</param>
		List<IResource> LoadResources(params string[] paths);

        /// <summary>
        /// Loads all the the resource available in the specified embedded/file system folder.
        /// </summary>
        /// <returns>The resources.</returns>
        /// <param name="folder">Folder.</param>
		List<IResource> LoadResources(string folder);

        /// <summary>
        /// Finds a file/resource path for the specified path.
        /// </summary>
        /// <returns>The file/resource path if found a matching resource/file, otherwise null.</returns>
        /// <param name="path">Path.</param>
        string FindFilePath(string path);
	}
}

