using System;
using System.Runtime.InteropServices;
using AGS.API;
using SkiaSharp;

namespace AGS.Engine
{
    public class FastSkiaBitmap : IDisposable
    {
        private byte[] _bytes;
        private SKBitmap _bitmap;
        private bool _isDirty;
        private SKPixmap _bitmapData;
        private int _bitmapWidth;
        private readonly int _bpp;

        public FastSkiaBitmap(SKBitmap bitmap, bool cleanSlate = false)
        {
            _bitmapWidth = bitmap.Width;
            _bpp = bitmap.BytesPerPixel;
            int byteCount = _bitmapWidth * bitmap.Height * _bpp;

            _bytes = new byte[byteCount];
            _bitmap = bitmap;

            _bitmapData = bitmap.PeekPixels();

            if (!cleanSlate)
                Marshal.Copy(_bitmapData.GetPixels(), _bytes, 0, byteCount);
        }

        public Color GetPixel(int x, int y)
        {
            int offset = getOffset(x, y);
            byte blue = _bytes[offset];
            byte green = _bytes[offset + 1];
            byte red = _bytes[offset + 2];
            byte alpha = _bytes[offset + 3];
            return Color.FromArgb(alpha, red, green, blue);
        }

        public bool IsOpaque(int x, int y)
        {
            int offset = getOffset(x, y);
            byte alpha = _bytes[offset + 3];
            return alpha == 255;
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
                var gcHandle = GCHandle.Alloc(_bytes, GCHandleType.Pinned);
                var info = new SKImageInfo(_bitmap.Width, _bitmap.Height, SKImageInfo.PlatformColorType, SKAlphaType.Unpremul);
                _bitmap.InstallPixels(info, gcHandle.AddrOfPinnedObject(), info.RowBytes, null, delegate { gcHandle.Free(); }, null);
            }
            _bitmapData.Dispose();
        }

        #endregion

        private int getOffset(int x, int y)
        {
            return (_bitmapWidth * y + x) * _bpp;
        }
    }
}