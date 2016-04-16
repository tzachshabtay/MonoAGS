using System;
using AGS.API;

namespace AGS.Engine
{
	public static class Hooks
	{
		public static IBrushLoader BrushLoader;
		public static IFontLoader FontLoader;
		public static IBitmapLoader BitmapLoader;
		public static IGameWindowSize GameWindowSize;
	}
}

