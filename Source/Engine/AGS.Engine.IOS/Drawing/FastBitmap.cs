extern alias IOS;

using System;
using System.Runtime.InteropServices;
using AGS.API;
using IOS::CoreGraphics;

namespace AGS.Engine.IOS
{
    public class FastBitmap : IDisposable
    {
        private int _width, _height, _bpp;
        private CGColorSpace _colorSpace;
        private CGBitmapContext _context;
        private IntPtr _bitmapData;

        public FastBitmap(CGImage bitmap, bool cleanSlate = false)
        {
            _width = (int)bitmap.Width;
            _height = (int)bitmap.Height;

            _colorSpace = CGColorSpace.CreateDeviceRGB();
            int bytesPerRow = (int)bitmap.BytesPerRow;
            _bpp = bytesPerRow / _width;
            int bitmapByteCount = bytesPerRow * _height;
            int bitsPerComponent = (int)bitmap.BitsPerComponent;
            CGImageAlphaInfo alphaInfo = CGImageAlphaInfo.PremultipliedLast;
            _bitmapData = Marshal.AllocHGlobal(bitmapByteCount);
            _context = new CGBitmapContext(_bitmapData, _width, _height, bitsPerComponent, bytesPerRow, _colorSpace, alphaInfo);
            _context.SetBlendMode(CGBlendMode.Copy);

            if (cleanSlate)
                _context.ClearRect(new CGRect(0f, 0f, _width, _height));
        }

        public Color GetPixel(int x, int y)
        {
            int offset = getOffset(x, y);
            byte blue = getByte(offset);
            byte green = getByte(offset + 1);
            byte red = getByte(offset + 2);
            byte alpha = getByte(offset + 3);
            return Color.FromRgba(red, green, blue, alpha);
        }

        public void SetPixel(int x, int y, Color color)
        {
            int offset = getOffset(x, y);
            setByte(offset, color.B);
            setByte(offset + 1, color.G);
            setByte(offset + 2, color.R);
            setByte(offset + 3, color.A);
        }

        #region IDisposable implementation

        public void Dispose()
        {
            _context.Dispose();
            _colorSpace.Dispose();
            Marshal.FreeHGlobal(_bitmapData);
        }

        #endregion

        private int getOffset(int x, int y)
        {
            return (_width * y + x) * _bpp;
        }

        private unsafe byte getByte(int offset) 
        {
            byte* bufferAsBytes = (byte*)_bitmapData;
            return bufferAsBytes[offset];
        }

        private unsafe void setByte(int offset, byte val)
        { 
            byte* bufferAsBytes = (byte*)_bitmapData;
            bufferAsBytes[offset] = val;
        }
    }
}
