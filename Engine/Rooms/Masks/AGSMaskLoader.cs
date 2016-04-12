using System;
using AGS.API;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AGS.Engine
{
	public class AGSMaskLoader : IMaskLoader
	{
		private IGameFactory _factory;

		public AGSMaskLoader(IGameFactory factory)
		{
			_factory = factory;
		}

		#region IMaskLoader implementation

		public IMask Load(string path, bool transparentMeansMasked = false, 
			IColor debugDrawColor = null, string saveMaskToFile = null, string id = null)
		{
			IBitmap image = new AGSBitmap((Bitmap)Image.FromFile(path)); 
			return load(path, image, transparentMeansMasked, debugDrawColor, saveMaskToFile, id);
		}

		public IMask Load(IBitmap image, bool transparentMeansMasked = false, 
			IColor debugDrawColor = null, string saveMaskToFile = null)
		{
			return load(null, image, transparentMeansMasked, debugDrawColor, saveMaskToFile);
		}

		#endregion

		private IMask load(string path, IBitmap image, bool transparentMeansMasked = false, 
			IColor debugDrawColor = null, string saveMaskToFile = null, string id = null)
		{
			return image.CreateMask(_factory, path, transparentMeansMasked, debugDrawColor, saveMaskToFile, id);
		}
	}
}

