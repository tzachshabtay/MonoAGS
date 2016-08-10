using System;
using AGS.API;
using System.Reflection;

namespace AGS.Engine
{
	public static class Hooks
	{
		public static IBrushLoader BrushLoader;
		public static IFontLoader FontLoader;
		public static IBitmapLoader BitmapLoader;
		public static IGameWindowSize GameWindowSize;
		public static IEngineConfigFile ConfigFile;
		public static Assembly EntryAssembly;
		public static IFileSystem FileSystem;
        public static IKeyboardState KeyboardState;
	}
}

