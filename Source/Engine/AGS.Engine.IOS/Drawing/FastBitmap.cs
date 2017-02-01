using System;
using System.Runtime.InteropServices;
using AGS.API;
using CoreGraphics;

namespace AGS.Engine.IOS
{
    public class FastBitmap : IDisposable
    {
        private byte[] _bytes;
        private bool _isDirty;
        private IntPtr _scan0;
        private CGImage _bitmap;
        const int BPP = 4;
        private int _width, _height;

        public FastBitmap(CGImage bitmap, bool cleanSlate = false)
        {
            _width = (int)bitmap.Width;
            _height = (int)bitmap.Height;
            int byteCount = _width * _height * BPP;

            _bytes = new byte[byteCount];
            _bitmap = bitmap;

            _scan0 = bitmap.Handle;

            if (!cleanSlate)
                Marshal.Copy(_scan0, _bytes, 0, byteCount);
        }

        public Color GetPixel(int x, int y)
        { 
            int offset = getOffset(x, y);
            byte blue = _bytes[offset];
            byte green = _bytes[offset + 1];
            byte red = _bytes[offset + 2];
            byte alpha = _bytes[offset + 3];
            return Color.FromRgba(red, green, blue, alpha);
        }

        public void SetPixel(int x, int y, Color color)
        {
            _isDirty = true;
            int offset = getOffset(x, y);
            _bytes[offset] = color.B;
            _bytes[offset + 1] = color.G;
            _bytes[offset + 2] = color.R;
            _bytes[offset + 3] = color.A;
        }

        #region IDisposable implementation

        public void Dispose()
        {
            if (_isDirty)
            {
                Marshal.Copy(_bytes, 0, _scan0, _bytes.Length);
            }
        }

        #endregion

        private int getOffset(int x, int y)
        {
            return (_width * y + x) * BPP;
        }
    }
}
