using System;
using System.Threading.Tasks;
using System.Drawing;

namespace API
{
	public interface IGraphicsFactory
	{
		IImage LoadImage(string filePath, ILoadImageConfig loadConfig = null);
		Task<IImage> LoadImageAsync(string filePath, ILoadImageConfig loadConfig = null);
		IImage LoadImage(Bitmap bitmap, ILoadImageConfig loadConfig = null, string id = null);

		IAnimation LoadAnimationFromFolder (string folderPath, int delay = 1, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);
		Task<IAnimation> LoadAnimationFromFolderAsync (string folderPath, int delay = 1, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);

		IAnimation LoadAnimationFromSpriteSheet (string filePath, ISpriteSheet spriteSheet, int delay = 1, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);
		Task<IAnimation> LoadAnimationFromSpriteSheetAsync (string filePath, ISpriteSheet spriteSheet, int delay = 1, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);
	}
}

