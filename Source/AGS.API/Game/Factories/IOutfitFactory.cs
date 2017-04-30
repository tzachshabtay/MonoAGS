using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// A factory for creating animation outfits for characters. The factory conveniently allows for idle, walk and speak animations,
    /// where other animations can be added to the outfit manually.
    /// </summary>
    public interface IOutfitFactory
	{
        /// <summary>
        /// Loads the outfit animation from several folders (each folder contains one animation).
        /// The folders can be resource folders, or actual folders on the file system (see cref="IRsourceLoader"/>.
        /// </summary>
        /// <returns>The outfit from folders.</returns>
        /// <param name="baseFolder">Base folder, all other folders will be on a relative path from this folder.</param>
        /// <param name="walkLeftFolder">Walk left folder.</param>
        /// <param name="walkRightFolder">Walk right folder.</param>
        /// <param name="walkDownFolder">Walk down folder.</param>
        /// <param name="walkUpFolder">Walk up folder.</param>
        /// <param name="idleLeftFolder">Idle left folder.</param>
        /// <param name="idleRightFolder">Idle right folder.</param>
        /// <param name="idleDownFolder">Idle down folder.</param>
        /// <param name="idleUpFolder">Idle up folder.</param>
        /// <param name="speakLeftFolder">Speak left folder.</param>
        /// <param name="speakRightFolder">Speak right folder.</param>
        /// <param name="speakDownFolder">Speak down folder.</param>
        /// <param name="speakUpFolder">Speak up folder.</param>
        /// <param name="animationConfig">Animation configuration to be applied for all animations.</param>
        /// <param name="loadConfig">Load image configuration to be applied for all images.</param>
		IOutfit LoadOutfitFromFolders(string baseFolder, string walkLeftFolder = null, string walkRightFolder = null,
			string walkDownFolder = null, string walkUpFolder = null, string idleLeftFolder = null, string idleRightFolder = null,
			string idleDownFolder = null, string idleUpFolder = null, string speakLeftFolder = null, string speakRightFolder = null,
			string speakDownFolder = null, string speakUpFolder = null,
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);

        /// <summary>
        /// Loads the outfit animation from several folders asynchronously (each folder contains one animation).
        /// The folders can be resource folders, or actual folders on the file system (<see cref="IResourceLoader"/>).
        /// </summary>
        /// <returns>The outfit from folders.</returns>
        /// <param name="baseFolder">Base folder, all other folders will be on a relative path from this folder.</param>
        /// <param name="walkLeftFolder">Walk left folder.</param>
        /// <param name="walkRightFolder">Walk right folder.</param>
        /// <param name="walkDownFolder">Walk down folder.</param>
        /// <param name="walkUpFolder">Walk up folder.</param>
        /// <param name="idleLeftFolder">Idle left folder.</param>
        /// <param name="idleRightFolder">Idle right folder.</param>
        /// <param name="idleDownFolder">Idle down folder.</param>
        /// <param name="idleUpFolder">Idle up folder.</param>
        /// <param name="speakLeftFolder">Speak left folder.</param>
        /// <param name="speakRightFolder">Speak right folder.</param>
        /// <param name="speakDownFolder">Speak down folder.</param>
        /// <param name="speakUpFolder">Speak up folder.</param>
        /// <param name="animationConfig">Animation configuration to be applied for all animations.</param>
        /// <param name="loadConfig">Load image configuration to be applied for all images.</param>
		Task<IOutfit> LoadOutfitFromFoldersAsync(string baseFolder, string walkLeftFolder = null, string walkRightFolder = null,
			string walkDownFolder = null, string walkUpFolder = null, string idleLeftFolder = null, string idleRightFolder = null,
			string idleDownFolder = null, string idleUpFolder = null, string speakLeftFolder = null, string speakRightFolder = null,
			string speakDownFolder = null, string speakUpFolder = null,
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);
	}
}

