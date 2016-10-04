namespace AGS.API
{
	public interface IBitmap
	{
		int Width { get; }
		int Height { get; }

		void Clear();
		Color GetPixel(int x, int y);
        void SetPixel(Color color, int x, int y);
		void MakeTransparent(Color color);
		void LoadTexture(int? textureToBind);
		IBitmap ApplyArea(IArea area);
		IBitmap Crop(Rectangle rectangle);
		IMask CreateMask(IGameFactory factory, string path, bool transparentMeansMasked = false, 
			Color? debugDrawColor = null, string saveMaskToFile = null, string id = null);
		IBitmapTextDraw GetTextDraw();
        void SaveToFile(string path);
    }
}

