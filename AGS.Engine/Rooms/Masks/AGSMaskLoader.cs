using System;
using AGS.API;


using System.Runtime.InteropServices;

namespace AGS.Engine
{
	public class AGSMaskLoader : IMaskLoader
	{
		private readonly IGameFactory _factory;
		private readonly IResourceLoader _resourceLoader;

		public AGSMaskLoader(IGameFactory factory, IResourceLoader resourceLoader)
		{
			_factory = factory;
			_resourceLoader = resourceLoader;
		}

		#region IMaskLoader implementation

		public IMask Load(string path, bool transparentMeansMasked = false, 
			Color? debugDrawColor = null, string saveMaskToFile = null, string id = null)
		{
			var resource = _resourceLoader.LoadResource (path);
			IBitmap image = Hooks.BitmapLoader.Load (resource.Stream);
			return load(path, image, transparentMeansMasked, debugDrawColor, saveMaskToFile, id);
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
			return image.CreateMask(_factory, path, transparentMeansMasked, debugDrawColor, saveMaskToFile, id);
		}
	}
}

