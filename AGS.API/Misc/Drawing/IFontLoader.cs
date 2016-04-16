using System;

namespace AGS.API
{
	public interface IFontLoader
	{
		IFont LoadFont(string fontFamily, float sizeInPoints, FontStyle style);
	}
}

