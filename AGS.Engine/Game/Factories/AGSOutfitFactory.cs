using System;
using System.Threading.Tasks;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public class AGSOutfitFactory : IOutfitFactory
	{
		private IContainer _resolver;
		private IGraphicsFactory _graphics;

		public AGSOutfitFactory(IContainer resolver, IGraphicsFactory graphics)
		{
			_resolver = resolver;
			_graphics = graphics;
		}

		public IOutfit LoadOutfitFromFolders(string baseFolder, string walkLeftFolder = null, string walkRightFolder = null,
			string walkDownFolder = null, string walkUpFolder = null, string idleLeftFolder = null, string idleRightFolder = null,
			string idleDownFolder = null, string idleUpFolder = null, string speakLeftFolder = null, string speakRightFolder = null,
			string speakDownFolder = null, string speakUpFolder = null,
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			IOutfit outfit = _resolver.Resolve<IOutfit>();

			outfit.IdleAnimation = _graphics.LoadDirectionalAnimationFromFolders(baseFolder, idleLeftFolder, idleRightFolder, 
				idleDownFolder, idleUpFolder, delay, animationConfig, loadConfig);

			outfit.WalkAnimation = _graphics.LoadDirectionalAnimationFromFolders(baseFolder, walkLeftFolder, walkRightFolder, 
				walkDownFolder, walkUpFolder, delay, animationConfig, loadConfig);

			outfit.SpeakAnimation = _graphics.LoadDirectionalAnimationFromFolders(baseFolder, speakLeftFolder, speakRightFolder, 
				speakDownFolder, speakUpFolder, delay, animationConfig, loadConfig);

			return outfit;
		}

		public async Task<IOutfit> LoadOutfitFromFoldersAsync(string baseFolder, string walkLeftFolder = null, string walkRightFolder = null,
			string walkDownFolder = null, string walkUpFolder = null, string idleLeftFolder = null, string idleRightFolder = null,
			string idleDownFolder = null, string idleUpFolder = null, string speakLeftFolder = null, string speakRightFolder = null,
			string speakDownFolder = null, string speakUpFolder = null,
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			IOutfit outfit = _resolver.Resolve<IOutfit> ();

			outfit.IdleAnimation = await _graphics.LoadDirectionalAnimationFromFoldersAsync(baseFolder, idleLeftFolder, idleRightFolder,
				idleDownFolder, idleUpFolder, delay, animationConfig, loadConfig);

			outfit.WalkAnimation = await _graphics.LoadDirectionalAnimationFromFoldersAsync (baseFolder, walkLeftFolder, walkRightFolder,
				walkDownFolder, walkUpFolder, delay, animationConfig, loadConfig);

			outfit.SpeakAnimation = await _graphics.LoadDirectionalAnimationFromFoldersAsync (baseFolder, speakLeftFolder, speakRightFolder,
				speakDownFolder, speakUpFolder, delay, animationConfig, loadConfig);

			return outfit;
		}
	}
}

