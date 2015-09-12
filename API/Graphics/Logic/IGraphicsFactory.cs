using System;
using System.Threading.Tasks;

namespace API
{
	public interface IGraphicsFactory
	{
		IImage LoadImage(string filePath);
		Task<IImage> LoadImageAsync(string filePath);

		IAnimation LoadAnimationFromFolder (string folderPath, int delay = 1, IAnimationConfiguration config = null);
		Task<IAnimation> LoadAnimationFromFolderAsync (string folderPath, int delay = 1, IAnimationConfiguration config = null);

		IAnimation LoadAnimationFromSpriteSheet (string filePath, ISpriteSheet spriteSheet, int delay = 1, IAnimationConfiguration config = null);
		Task<IAnimation> LoadAnimationFromSpriteSheetAsync (string filePath, ISpriteSheet spriteSheet, int delay = 1, IAnimationConfiguration config = null);
	}
}

