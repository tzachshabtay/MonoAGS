using System;

namespace AGS.API
{
	public interface IFontLoader
	{
		void InstallFonts(params string[] paths);
		IFont LoadFont(string fontFamily, float sizeInPoints, FontStyle style = FontStyle.Regular);
		IFont LoadFontFromPath(string path, float sizeInPoints, FontStyle style = FontStyle.Regular);
	}
}

