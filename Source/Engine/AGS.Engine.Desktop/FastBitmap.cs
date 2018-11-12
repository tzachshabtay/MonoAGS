using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace AGS.Engine.Desktop
{
	public class FastBitmap : IDisposable
	{
		private byte[] _bytes;
		private Bitmap _bitmap;
		private bool _isDirty;
		private BitmapData _bitmapData;
        private int _bitmapWidth;
		const int bpp = 4;

		public FastBitmap(Bitmap bitmap, ImageLockMode lockMode, bool cleanSlate = false)
		{
            _bitmapWidth = bitmap.Width;
			int byteCount = _bitmapWidth * bitmap.Height * bpp;

			_bytes  = new byte[byteCount];
			_bitmap = bitmap;

			_bitmapData = bitmap.LockBits(new Rectangle(0, 0, _bitmapWidth, bitmap.Height), lockMode, 
				PixelFormat.Format32bppArgb);

			if (!cleanSlate)
				Marshal.Copy(_bitmapData.Scan0, _bytes, 0, byteCount);
		}
			
		public Color GetPixel(int x, int y)
		{
			int offset = getOffset(x, y);
			byte blue  = _bytes[offset];
			byte green = _bytes[offset + 1];
			byte red   = _bytes[offset + 2];
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
			_bytes[offset]     = color.B;
			_bytes[offset + 1] = color.G;
			_bytes[offset + 2] = color.R;
			_bytes[offset + 3] = color.A;
		}

		#region IDisposable implementation

		public void Dispose()
		{
			if (_isDirty)
			{
				Marshal.Copy(_bytes, 0, _bitmapData.Scan0, _bytes.Length);
			}
			_bitmap.UnlockBits(_bitmapData);
		}

		#endregion

		private int getOffset(int x, int y)
		{
			return (_bitmapWidth * y + x) * bpp;
		}
	}
}

