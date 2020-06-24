using AGS.API;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

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
            Debug.WriteLine("MaskLoader: Load " + (path ?? "null"));
            var resource = _resourceLoader.LoadResource(path);
            IBitmap image = _bitmapLoader.Load(resource.Stream);
            return load(path, image, transparentMeansMasked, debugDrawColor, saveMaskToFile, id);
        }

        public IMask Load(bool[,] mask, string id, bool inverseMask = false, Color? debugDrawColor = null, string saveMaskToFile = null)
        {
            IBitmap image = _bitmapLoader.Load(mask.GetLength(0), mask.GetLength(1));
            List<Point> points = new List<Point>(image.Width * image.Height);
            for (int row = 0; row < mask.GetLength(1); row++)
            {
                for (int col = 0; col < mask.GetLength(0); col++)
                {
                    if (mask[col, row])
                    {
                        if (!inverseMask) points.Add(new Point(col, row));
                    }
                    else if (inverseMask) points.Add(new Point(col, row));
                }
            }
            image.SetPixels(Colors.Black, points);
            return load(null, image, false, debugDrawColor, saveMaskToFile, id);
        }

        public async Task<IMask> LoadAsync(string path, bool transparentMeansMasked = false,
            Color? debugDrawColor = null, string saveMaskToFile = null, string id = null)
        {
            Debug.WriteLine("MaskLoader: LoadAsync " + (path ?? "null"));
            var resource = await Task.Run(() => _resourceLoader.LoadResource(path));
            if (resource == null) return null;
            IBitmap image = await Task.Run(() => _bitmapLoader.Load(resource.Stream));
            return load(path, image, transparentMeansMasked, debugDrawColor, saveMaskToFile, id);
        }

        public IMask Load(string id, IBitmap image, bool transparentMeansMasked = false,
            Color? debugDrawColor = null, string saveMaskToFile = null)
        {
            return load(null, image, transparentMeansMasked, debugDrawColor, saveMaskToFile, id);
        }

        #endregion

        private IMask load(string path, IBitmap image, bool transparentMeansMasked = false,
            Color? debugDrawColor = null, string saveMaskToFile = null, string id = null)
        {
#if DEBUG
            bool hasColor = debugDrawColor != null;
            debugDrawColor = debugDrawColor ?? Colors.Blue.WithAlpha(150); //for the debug display list window
#endif
            var mask = image.CreateMask(_factory, path, transparentMeansMasked, debugDrawColor, saveMaskToFile, id);
            if (mask.DebugDraw != null) mask.DebugDraw.Enabled = false;
#if DEBUG
            if (!hasColor && mask.DebugDraw != null) mask.DebugDraw.Visible = false;
#endif
            return mask;
		}
	}
}

