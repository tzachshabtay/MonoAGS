using System;
using AGS.API;

using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Drawing;

namespace AGS.Engine.Desktop
{
	public class DesktopFontFamilyLoader
	{
		private IResourceLoader _resources;
        private PrivateFontCollection _fontCollection;
        private Dictionary<string, FontFamily> _families;
        private HashSet<string> _loadedFamilies;
		private Action _refreshFontCache;

		private const string MAC_FONT_LIBRARY = "/Library/Fonts";
        private readonly string LINUX_FONT_LIBRARY = $"{Environment.GetEnvironmentVariable("HOME")}/.fonts"; //or: /usr/share/fonts/(truetype)

		public DesktopFontFamilyLoader(IResourceLoader resources)
		{
			_resources = resources;
            _fontCollection = new PrivateFontCollection();
            _loadedFamilies = new HashSet<string>();
            _families = new Dictionary<string, FontFamily>();
		}

		public void InstallFonts(params string[] paths)
		{
			foreach (string path in paths)
			{
				LoadFontFamily(path);
			}
            _refreshFontCache?.Invoke();
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

			_fontCollection.AddMemoryFont(fontPtr, buffer.Length);

            //Marshal.FreeCoTaskMem(fontPtr); The pointer should not be released: See Hans Passant's answer on this: http://stackoverflow.com/questions/25583394/privatefontcollection-addmemoryfont-producing-random-errors-on-windows-server-20

            FontFamily family = null;
            bool foundFamily = false;
            for (int i = 0; i < _fontCollection.Families.Length; i++)
            {
                family = _fontCollection.Families[i];
                if (_loadedFamilies.Add(family.Name))
                {
                    foundFamily = true;
                    break;
                }
            }
            if (!foundFamily)
            {
                throw new IOException($"Failed to find family from {path} after adding it to the private memory collection.");
            }

			if (isMac())
			{
				return loadFontFamilyOnMac(path, buffer, family);
			}
            else if (isLinux())
            {
                return loadFontFamilyOnLinux(path, buffer, family);
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
                _refreshFontCache = restart;
			}
			return new FontFamily (family.Name);
		}

        //https://wiki.ubuntu.com/Fonts
        private FontFamily loadFontFamilyOnLinux(string path, byte[] buffer, FontFamily family)
        {
            string filename = Path.GetFileName(path);
            path = Path.Combine(LINUX_FONT_LIBRARY, filename);
            if (!Directory.Exists(LINUX_FONT_LIBRARY))
            {
                Directory.CreateDirectory(LINUX_FONT_LIBRARY);
            }
            if (!File.Exists(path))
            {
                File.WriteAllBytes(path, buffer);
                _refreshFontCache = () => {
                    Debug.WriteLine("Refreshing font cache");
                    ProcessStartInfo info = new ProcessStartInfo("fc-cache", "-f -v") { UseShellExecute = false };
                    var process = Process.Start(info);
                    process.WaitForExit();
                    Debug.WriteLine("Completed refreshing font cache, restarting...");
                    restart();
                };
            }
            return new FontFamily(family.Name);
        }

		private bool isMac()
		{
			if (Path.DirectorySeparatorChar != '/') return false;
			return Directory.Exists(MAC_FONT_LIBRARY);
		}

        private bool isLinux()
        {
            return Path.DirectorySeparatorChar == '/';
        }

        private void restart()
        {
            //todo: If we can't find a better way, at least show a message box that we're gonna attempt to restart as new fonts were installed
            string processPath = Assembly.GetEntryAssembly().CodeBase;
            ProcessStartInfo info = new ProcessStartInfo("mono", processPath) { UseShellExecute = false };
            Process.Start(info);
            Environment.Exit(0);
        }
	}
}

