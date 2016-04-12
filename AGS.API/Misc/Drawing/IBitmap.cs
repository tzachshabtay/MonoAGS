using System;

namespace AGS.API
{
	public interface IBitmap
	{
		int Width { get; }
		int Height { get; }

		void Clear();
		IColor GetPixel(int x, int y);
		void MakeTransparent(IColor color);
		void LoadTexture(int? textureToBind);
		IBitmap ApplyArea(IArea area);
		IMask CreateMask(IGameFactory factory, string path, bool transparentMeansMasked = false, 
			IColor debugDrawColor = null, string saveMaskToFile = null, string id = null);
		IBitmapTextDraw GetTextDraw();
	}
}

