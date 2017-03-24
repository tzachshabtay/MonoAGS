extern alias IOS;

using System;
using System.Runtime.InteropServices;
using AGS.API;
using IOS::CoreGraphics;
using IOS::UIKit;

namespace AGS.Engine.IOS
{
    public class FastBitmap : IDisposable
    {
        private int _width, _height, _bpp;
        private CGColorSpace _colorSpace;
        private CGBitmapContext _context;
        private byte[] _bitmapData;
        private GCHandle _handle;

        public FastBitmap(int width, int height)
        {
            _width = width;
            _height = height;

            _colorSpace = CGColorSpace.CreateDeviceRGB();
            _bpp = 4;
            int bytesPerRow = _width * _bpp;
            int bitmapByteCount = bytesPerRow * _height;
            const int bitsPerComponent = 8;
            CGBitmapFlags flags = CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big;

            _bitmapData = new byte[bitmapByteCount];
            _handle = GCHandle.Alloc(_bitmapData);

            _context = new CGBitmapContext(_bitmapData, _width, _height, bitsPerComponent, bytesPerRow, _colorSpace, flags);
        }

        public FastBitmap(CGImage bitmap) : this((int)bitmap.Width, (int)bitmap.Height)
        { 
            _context.DrawImage(new CGRect(0f, 0f, _width, _height), bitmap);
        }

        public Color GetPixel(int x, int y)
        {
            int offset = getOffset(x, y);
            byte red = _bitmapData[offset];
            byte green = _bitmapData[offset + 1];
            byte blue = _bitmapData[offset + 2];
            byte alpha = _bitmapData[offset + 3];
            return Color.FromRgba(red, green, blue, alpha);
        }

        public void SetPixel(int x, int y, Color color)
        {
            int offset = getOffset(x, y);
            _bitmapData[offset] = color.R;
            _bitmapData[offset + 1] = color.G;
            _bitmapData[offset + 2] = color.B;
            _bitmapData[offset + 3] = color.A;
        }

        public UIImage GetImage()
        {
            return UIImage.FromImage(_context.ToImage());
        }

        #region IDisposable implementation

        public void Dispose()
        {
            _context.Dispose();
            _colorSpace.Dispose();
            _handle.Free();
        }

        #endregion

        private int getOffset(int x, int y)
        {
            return (_width * (_height - y - 1) + x) * _bpp;
        }
    }
}
