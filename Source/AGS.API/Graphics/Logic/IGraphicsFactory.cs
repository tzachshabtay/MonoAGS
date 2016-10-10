using System.Threading.Tasks;

namespace AGS.API
{
    public interface IGraphicsFactory
	{
		ISprite GetSprite();

		IImage LoadImage(string filePath, ILoadImageConfig loadConfig = null);
		Task<IImage> LoadImageAsync(string filePath, ILoadImageConfig loadConfig = null);
		IImage LoadImage(IBitmap bitmap, ILoadImageConfig loadConfig = null, string id = null);

		IDirectionalAnimation LoadDirectionalAnimationFromFolders(string baseFolder, string leftFolder = null,
		                                                          string rightFolder = null, string downFolder = null, string upFolder = null,
		                                                          IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);
		Task<IDirectionalAnimation> LoadDirectionalAnimationFromFoldersAsync(string baseFolder, string leftFolder = null,
																  string rightFolder = null, string downFolder = null, string upFolder = null,
																  IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);

		IAnimation LoadAnimationFromFiles(IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null, params string[] files);
		Task<IAnimation> LoadAnimationFromFilesAsync(IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null, params string [] files);

		IAnimation LoadAnimationFromFolder (string folderPath, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);
		Task<IAnimation> LoadAnimationFromFolderAsync (string folderPath, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);

		IAnimation LoadAnimationFromSpriteSheet (ISpriteSheet spriteSheet, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);
		Task<IAnimation> LoadAnimationFromSpriteSheetAsync (ISpriteSheet spriteSheet, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);
	}
}

