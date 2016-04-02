using System;
using AGS.API;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class AGSFontLoader
	{
		private IResourceLoader _resources;
        private PrivateFontCollection _fontCollection;
        private Dictionary<string, FontFamily> _families;

		public AGSFontLoader(IResourceLoader resources)
		{
			_resources = resources;
            _fontCollection = new PrivateFontCollection();
            _families = new Dictionary<string, FontFamily>();
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

			//todo: this only works on windows: http://stackoverflow.com/questions/32406859/privatefontcollection-in-mono-3-2-8-on-linux-ubuntu-14-04-1
			_fontCollection.AddMemoryFont(fontPtr, buffer.Length);

			Marshal.FreeCoTaskMem(fontPtr);

			return _fontCollection.Families[_fontCollection.Families.Length - 1];
			/*var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			/*try 
			{
			    var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
				PrivateFontCollection fontCollection = new PrivateFontCollection();
				fontCollection.AddMemoryFont(ptr, buffer.Length);
			    return fontCollection.Families[0];
			} 
			finally 
			{
				// don't forget to unpin the array!
				handle.Free();
			}*/
		}
	}
}

