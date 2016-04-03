using System;
using AGS.API;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace AGS.Engine
{
	public class AGSFontLoader
	{
		private IResourceLoader _resources;
        private PrivateFontCollection _fontCollection;
		private int _lastFontInCollection;
        private Dictionary<string, FontFamily> _families;
		private bool _restartNeeded;

		private const string MAC_FONT_LIBRARY = "/Library/Fonts";

		public AGSFontLoader(IResourceLoader resources)
		{
			_resources = resources;
            _fontCollection = new PrivateFontCollection();
            _families = new Dictionary<string, FontFamily>();
		}

		public void InstallFonts(params string[] paths)
		{
			foreach (string path in paths)
			{
				LoadFontFamily(path);
			}
			if (!_restartNeeded) return;

			//todo: If we can't find a better way, at least show a message box that we're gonna attempt to restart as new fonts were installed
			string processPath = Assembly.GetEntryAssembly().CodeBase;
			ProcessStartInfo info = new ProcessStartInfo("mono", processPath) { UseShellExecute = false };
			Process.Start(info);
			Environment.Exit(0);
		}

        public FontFamily LoadFontFamily(string path)
        {
            return _families.GetOrAdd(path, () => loadFontFamily(path));
        }

		private FontFamily loadFontFamily(string path)
		{
			IResource resource = _resources.LoadResource(path);

			var buffer = new byte[resource.Stream.Length];
			resource.Stream.Read(buffer, 0, buffer.Length);

			IntPtr fontPtr = Marshal.AllocCoTaskMem(buffer.Length);

			Marshal.Copy(buffer, 0, fontPtr, buffer.Length);

			//todo: make this work on linux: http://stackoverflow.com/questions/32406859/privatefontcollection-in-mono-3-2-8-on-linux-ubuntu-14-04-1
			_fontCollection.AddMemoryFont(fontPtr, buffer.Length);

			//Marshal.FreeCoTaskMem(fontPtr); The pointer should not be released: See Hans Passant's answer on this: http://stackoverflow.com/questions/25583394/privatefontcollection-addmemoryfont-producing-random-errors-on-windows-server-20

			FontFamily family = _fontCollection.Families[_lastFontInCollection];
			_lastFontInCollection++;

			if (isMac())
			{
				return loadFontFamilyOnMac(path, buffer, family);
			}

			return family;
		}

		//How to install fonts: http://www.dafont.com/faq.php
		private FontFamily loadFontFamilyOnMac(string path, byte[] buffer, FontFamily family)
		{
			string filename = Path.GetFileName(path);
			path = Path.Combine(MAC_FONT_LIBRARY, filename);
			if (!File.Exists(path))
			{
				File.WriteAllBytes(path, buffer);
				//todo: installing a font on a mac doesn't catch unless you restart.. need to find a better way
				_restartNeeded = true;
			}
			return new FontFamily (family.Name);
		}

		private bool isMac()
		{
			if (Path.DirectorySeparatorChar != '/') return false;
			return Directory.Exists(MAC_FONT_LIBRARY);
		}
	}
}

