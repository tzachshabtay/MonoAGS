using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// Factory to allow loading graphics (images/animations).
    /// </summary>
    public interface IGraphicsFactory
	{
        /// <summary>
        /// Factory for creating icons.
        /// </summary>
        /// <value>The icons.</value>
        IIconFactory Icons { get; }

        /// <summary>
        /// Factory for creating brushes.
        /// </summary>
        /// <value>The brushes.</value>
        IBrushLoader Brushes { get; }

        /// <summary>
        /// Creates a new sprite
        /// </summary>
        /// <returns>The sprite.</returns>
		ISprite GetSprite();

        /// <summary>
        /// Loads an image from a resource/file path (<see cref="IResourceLoader"/>).
        /// </summary>
        /// <returns>The image.</returns>
        /// <param name="filePath">File/resource path.</param>
        /// <param name="loadConfig">Configuration on how to load the image.</param>
		IImage LoadImage(string filePath, ILoadImageConfig loadConfig = null);

        /// <summary>
        /// Loads an image asynchronously from a resource/file path (<see cref="IResourceLoader"/>).
        /// </summary>
        /// <returns>The image.</returns>
        /// <param name="filePath">File/resource path.</param>
        /// <param name="loadConfig">Configuration on how to load the image.</param>
		Task<IImage> LoadImageAsync(string filePath, ILoadImageConfig loadConfig = null);

        /// <summary>
        /// Loads an image from a bitmap.
        /// </summary>
        /// <returns>The image.</returns>
        /// <param name="loadConfig">Configuration on how to load the image.</param>
        /// <param name="id">An optional unique id for the image (if not given, the engine will generate one automatically).</param>
		IImage LoadImage(IBitmap bitmap, ILoadImageConfig loadConfig = null, string id = null);

        /// <summary>
        /// Loads a directional animation from file/resource folders (<see cref="IResourceLoader"/> .
        /// </summary>
        /// <returns>The directional animation.</returns>
        /// <param name="baseFolder">A base folder, from which all other folders will be relative to.</param>
        /// <param name="leftFolder">Left animation folder (null to not have a left animation).</param>
        /// <param name="rightFolder">Right animation folder (null to not have a right animation).</param>
        /// <param name="downFolder">Down animation folder (null to not have a down animation).</param>
        /// <param name="upFolder">Up animation folder (null to not have an up animation).</param>
        /// <param name="animationConfig">Animation configuration (will be applied to all directions).</param>
        /// <param name="loadConfig">Configuration for loading the images (will be applied to all loaded images).</param>
		IDirectionalAnimation LoadDirectionalAnimationFromFolders(string baseFolder, string leftFolder = null,
		                                                          string rightFolder = null, string downFolder = null, string upFolder = null,
		                                                          IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);

        /// <summary>
        /// Loads a directional animation asynchronously from file/resource folders (<see cref="IResourceLoader"/> .
        /// </summary>
        /// <returns>The directional animation.</returns>
        /// <param name="baseFolder">A base folder, from which all other folders will be relative to.</param>
        /// <param name="leftFolder">Left animation folder (null to not have a left animation).</param>
        /// <param name="rightFolder">Right animation folder (null to not have a right animation).</param>
        /// <param name="downFolder">Down animation folder (null to not have a down animation).</param>
        /// <param name="upFolder">Up animation folder (null to not have an up animation).</param>
        /// <param name="animationConfig">Animation configuration (will be applied to all directions).</param>
        /// <param name="loadConfig">Configuration for loading the images (will be applied to all loaded images).</param>
		Task<IDirectionalAnimation> LoadDirectionalAnimationFromFoldersAsync(string baseFolder, string leftFolder = null,
																  string rightFolder = null, string downFolder = null, string upFolder = null,
																  IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);

        /// <summary>
        /// Loads an animation from a list of resource/file paths (<see cref="IResourceLoader"/>).
        /// The order of the files/resources will decide the order of the animation.
        /// </summary>
        /// <returns>The animation.</returns>
        /// <param name="animationConfig">Animation configuration.</param>
        /// <param name="loadConfig">Configuration for how to load the images.</param>
        /// <param name="files">A list of resource/file paths.</param>
		IAnimation LoadAnimationFromFiles(IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null, params string[] files);

        /// <summary>
        /// Loads an animation asynchronously from a list of resource/file paths (<see cref="IResourceLoader"/>).
        /// The order of the files/resources will decide the order of the animation.
        /// </summary>
        /// <returns>The animation.</returns>
        /// <param name="animationConfig">Animation configuration.</param>
        /// <param name="loadConfig">Configuration for how to load the images.</param>
        /// <param name="files">A list of resource/file paths.</param>
		Task<IAnimation> LoadAnimationFromFilesAsync(IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null, params string [] files);

        /// <summary>
        /// Loads an animation from a resource/file folder (<see cref="IResourceLoader"/>).
        /// The files/resources will be loaded in an alphabetical order which will determine the order of the animation.
        /// So if you list your files in the folder, for example: walk1.png, walk2.png, walk3.png, the animation
        /// will be ordered accordingly.
        /// </summary>
        /// <returns>The animation.</returns>
        /// <param name="folderPath">The resource/file folder path.</param>
        /// <param name="animationConfig">Animation configuration.</param>
        /// <param name="loadConfig">Configuration for how to load the images.</param>
		IAnimation LoadAnimationFromFolder (string folderPath, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);

        /// <summary>
        /// Loads an animation asynchronously from a resource/file folder (<see cref="IResourceLoader"/>).
        /// The files/resources will be loaded in an alphabetical order which will determine the order of the animation.
        /// So if you list your files in the folder, for example: walk1.png, walk2.png, walk3.png, the animation
        /// will be ordered accordingly.
        /// </summary>
        /// <returns>The animation.</returns>
        /// <param name="folderPath">The resource/file folder path.</param>
        /// <param name="animationConfig">Animation configuration.</param>
        /// <param name="loadConfig">Configuration for how to load the images.</param>
		Task<IAnimation> LoadAnimationFromFolderAsync (string folderPath, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);

        /// <summary>
        /// Loads an animation from a sprite sheet.
        /// </summary>
        /// <returns>The animation.</returns>
        /// <param name="spriteSheet">Sprite sheet.</param>
        /// <param name="animationConfig">Animation configuration.</param>
        /// <param name="loadConfig">Configuration for how to load the images.</param>
		IAnimation LoadAnimationFromSpriteSheet (ISpriteSheet spriteSheet, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);

        /// <summary>
        /// Loads an animation asynchronously from a sprite sheet.
        /// </summary>
        /// <returns>The animation.</returns>
        /// <param name="spriteSheet">Sprite sheet.</param>
        /// <param name="animationConfig">Animation configuration.</param>
        /// <param name="loadConfig">Configuration for how to load the images.</param>
		Task<IAnimation> LoadAnimationFromSpriteSheetAsync (ISpriteSheet spriteSheet, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);
	}
}

