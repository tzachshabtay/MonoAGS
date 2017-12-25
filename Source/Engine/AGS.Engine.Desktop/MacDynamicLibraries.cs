using System;
using System.Runtime.InteropServices;

namespace AGS.Engine.Desktop
{
    //code from: https://github.com/mono/SkiaSharp/blob/a7909ce464659e097b5e86c5df48084d0d2b6458/tests/Tests/SKTest.cs
    public static class MacDynamicLibraries
    {
        private const string SystemLibrary = "/usr/lib/libSystem.dylib";
        [DllImport(SystemLibrary)]
        public static extern IntPtr dlopen(string path, int mode);
        [DllImport(SystemLibrary)]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);
        [DllImport(SystemLibrary)]
        public static extern void dlclose(IntPtr handle);
    }
}
