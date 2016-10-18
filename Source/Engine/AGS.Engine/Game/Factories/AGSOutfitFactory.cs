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
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			IOutfit outfit = _resolver.Resolve<IOutfit>();

            outfit[AGSOutfit.Idle] = _graphics.LoadDirectionalAnimationFromFolders(baseFolder, idleLeftFolder, idleRightFolder, 
				idleDownFolder, idleUpFolder, animationConfig, loadConfig);

            outfit[AGSOutfit.Walk] = _graphics.LoadDirectionalAnimationFromFolders(baseFolder, walkLeftFolder, walkRightFolder, 
				walkDownFolder, walkUpFolder, animationConfig, loadConfig);

            outfit[AGSOutfit.Speak] = _graphics.LoadDirectionalAnimationFromFolders(baseFolder, speakLeftFolder, speakRightFolder, 
				speakDownFolder, speakUpFolder, animationConfig, loadConfig);

			return outfit;
		}

		public async Task<IOutfit> LoadOutfitFromFoldersAsync(string baseFolder, string walkLeftFolder = null, string walkRightFolder = null,
			string walkDownFolder = null, string walkUpFolder = null, string idleLeftFolder = null, string idleRightFolder = null,
			string idleDownFolder = null, string idleUpFolder = null, string speakLeftFolder = null, string speakRightFolder = null,
			string speakDownFolder = null, string speakUpFolder = null,
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			IOutfit outfit = _resolver.Resolve<IOutfit> ();

            outfit[AGSOutfit.Idle] = await _graphics.LoadDirectionalAnimationFromFoldersAsync(baseFolder, idleLeftFolder, idleRightFolder,
				idleDownFolder, idleUpFolder, animationConfig, loadConfig);

            outfit[AGSOutfit.Walk] = await _graphics.LoadDirectionalAnimationFromFoldersAsync (baseFolder, walkLeftFolder, walkRightFolder,
				walkDownFolder, walkUpFolder, animationConfig, loadConfig);

            outfit[AGSOutfit.Speak] = await _graphics.LoadDirectionalAnimationFromFoldersAsync (baseFolder, speakLeftFolder, speakRightFolder,
				speakDownFolder, speakUpFolder, animationConfig, loadConfig);

			return outfit;
		}
	}
}

