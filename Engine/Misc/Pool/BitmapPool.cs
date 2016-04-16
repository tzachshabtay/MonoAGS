using System;
using System.Collections.Concurrent;

using AGS.API;

namespace AGS.Engine
{
	public class BitmapPool
	{
		private ConcurrentDictionary<AGS.API.Size, ObjectPool<IBitmap>> _bitmaps;

		public BitmapPool()
		{
			_bitmaps = new ConcurrentDictionary<AGS.API.Size, ObjectPool<IBitmap>> (2, 40);
			initPool();		
		}

		public IBitmap Acquire(int width, int height)
		{
			width = MathUtils.GetNextPowerOf2(width);
			height = MathUtils.GetNextPowerOf2(height);
			return getPool(width, height).Acquire();
		}

		public void Release(IBitmap bitmap)
		{
			if (!MathUtils.IsPowerOf2(bitmap.Width) ||
			    !MathUtils.IsPowerOf2(bitmap.Height)) return;

			getPool(bitmap.Width, bitmap.Height).Release(bitmap);
		}

		private void initPool()
		{
			//This pool will be mainly used for texts.
			//Optimizing the default pool with the thought that most texts will
			//need big widths and small heights.

			int height = 8;
			while (height <= 1024)
			{
				int width = height;
				while (width <= 1024)
				{
					getPool(width, height);
					width *= 2;
				}
				height *= 2;
			}
		}

		private ObjectPool<IBitmap> getPool(int width, int height)
		{
			AGS.API.Size size = new AGS.API.Size (width, height);
			return _bitmaps.GetOrAdd(size, _ => new ObjectPool<IBitmap> (() => Hooks.BitmapLoader.Load(width, height), 3,
				bitmap => bitmap.Clear()));
		}

	}
}

