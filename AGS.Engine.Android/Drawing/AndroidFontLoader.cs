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

		public IFont LoadFont(string fontFamily, float sizeInPoints, FontStyle style)
		{
			return new AndroidFont (fontFamily, style, sizeInPoints);
		}

		#endregion
	}
}

