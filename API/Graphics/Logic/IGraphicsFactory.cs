using System;
using System.Threading.Tasks;
using System.Drawing;

namespace AGS.API
{
	public interface IGraphicsFactory
	{
		ISprite GetSprite();

		IImage LoadImage(string filePath, ILoadImageConfig loadConfig = null);
		Task<IImage> LoadImageAsync(string filePath, ILoadImageConfig loadConfig = null);
		IImage LoadImage(Bitmap bitmap, ILoadImageConfig loadConfig = null, string id = null);

		IAnimation LoadAnimationFromFiles(int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null, params string[] files);

		IAnimation LoadAnimationFromFolder (string folderPath, int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);
		Task<IAnimation> LoadAnimationFromFolderAsync (string folderPath, int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);

		IAnimation LoadAnimationFromSpriteSheet (string filePath, ISpriteSheet spriteSheet, int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);
		Task<IAnimation> LoadAnimationFromSpriteSheetAsync (string filePath, ISpriteSheet spriteSheet, int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);
	}
}

