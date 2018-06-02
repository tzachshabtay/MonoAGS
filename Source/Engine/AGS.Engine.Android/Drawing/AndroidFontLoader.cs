using System;
using AGS.API;

namespace AGS.Engine.Android
{
	public class AndroidFontLoader : IFontLoader
	{
		public AndroidFontLoader()
		{
		}

		#region IFontLoader implementation

		public void InstallFonts(params string[] paths)
		{
		}
			
		public IFont LoadFont(string fontFamily, float sizeInPoints, FontStyle style)
		{
			return AndroidFont.FromFamilyName(fontFamily, style, sizeInPoints, this);
		}
			
		public IFont LoadFontFromPath(string path, float sizeInPoints, FontStyle style)
		{
			return AndroidFont.FromPath(path, style, sizeInPoints, this);
		}

		#endregion
	}
}