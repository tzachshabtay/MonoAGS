using System;
using AGS.API;
using System.Drawing;

namespace AGS.Engine.Desktop
{
	public class DesktopFontLoader : IFontLoader
	{
		public DesktopFontLoader()
		{
		}

		#region IFontLoader implementation

		public IFont LoadFont(string fontFamily, float sizeInPoints, AGS.API.FontStyle style)
		{
			return new DesktopFont (new Font (fontFamily ?? SystemFonts.DefaultFont.FontFamily.Name, sizeInPoints, style.Convert()));
		}

		#endregion
	}
}

