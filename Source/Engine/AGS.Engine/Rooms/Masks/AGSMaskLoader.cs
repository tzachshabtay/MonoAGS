using AGS.API;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AGS.Engine
{
	public class AGSMaskLoader : IMaskLoader
	{
		private readonly IGameFactory _factory;
		private readonly IResourceLoader _resourceLoader;
        private readonly IBitmapLoader _bitmapLoader;

        public AGSMaskLoader(IGameFactory factory, IResourceLoader resourceLoader, IBitmapLoader bitmapLoader)
		{
			_factory = factory;
			_resourceLoader = resourceLoader;
            _bitmapLoader = bitmapLoader;
		}

		#region IMaskLoader implementation

		public IMask Load(string path, bool transparentMeansMasked = false, 
			Color? debugDrawColor = null, string saveMaskToFile = null, string id = null)
		{
            Debug.WriteLine("MaskLoader: Load " + path ?? "null");
			var resource = _resourceLoader.LoadResource (path);
			IBitmap image = _bitmapLoader.Load (resource.Stream);
			return load(path, image, transparentMeansMasked, debugDrawColor, saveMaskToFile, id);
		}

		public async Task<IMask> LoadAsync (string path, bool transparentMeansMasked = false,
			Color? debugDrawColor = null, string saveMaskToFile = null, string id = null)
		{
            Debug.WriteLine("MaskLoader: LoadAsync " + path ?? "null");
			var resource = await Task.Run(() => _resourceLoader.LoadResource (path));
            if (resource == null) return null;
			IBitmap image = await Task.Run(() => _bitmapLoader.Load (resource.Stream));
			return load (path, image, transparentMeansMasked, debugDrawColor, saveMaskToFile, id);
		}

		public IMask Load(IBitmap image, bool transparentMeansMasked = false, 
			Color? debugDrawColor = null, string saveMaskToFile = null)
		{
			return load(null, image, transparentMeansMasked, debugDrawColor, saveMaskToFile);
		}

		#endregion

		private IMask load(string path, IBitmap image, bool transparentMeansMasked = false, 
			Color? debugDrawColor = null, string saveMaskToFile = null, string id = null)
		{
			var mask = image.CreateMask(_factory, path, transparentMeansMasked, debugDrawColor, saveMaskToFile, id);
            return mask;
		}
	}
}

