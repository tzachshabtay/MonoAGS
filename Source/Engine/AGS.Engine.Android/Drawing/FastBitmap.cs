using System;
using Android.Graphics;
using System.Runtime.InteropServices;

namespace AGS.Engine.Android
{
	public class FastBitmap : IDisposable
	{
		private byte[] _bytes;
		private Bitmap _bitmap;
		private bool _isDirty;
		private IntPtr _scan0;
        const int BPP = 4;

		public FastBitmap(Bitmap bitmap, bool cleanSlate = false)
		{
			int byteCount = bitmap.Width * bitmap.Height * BPP;

			_bytes  = new byte[byteCount];
			_bitmap = bitmap;

			_scan0 = bitmap.LockPixels();

			if (!cleanSlate)
				Marshal.Copy(_scan0, _bytes, 0, byteCount);
		}

		public Color GetPixel(int x, int y)
		{
			int offset = getOffset(x, y);
			byte blue  = _bytes[offset];
			byte green = _bytes[offset + 1];
			byte red   = _bytes[offset + 2];
			byte alpha = _bytes[offset + 3];
			return new Color (red, green, blue, alpha);
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
				Marshal.Copy(_bytes, 0, _scan0, _bytes.Length);
			}
			_bitmap.UnlockPixels();
		}

		#endregion

		private int getOffset(int x, int y)
		{
			return (_bitmap.Width * y + x) * BPP;
		}
	}
}

