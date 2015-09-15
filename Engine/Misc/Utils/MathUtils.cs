using System;

namespace AGS.Engine
{
	public static class MathUtils
	{
		public static float Lerp(float x1, float y1, float x2, float y2, float targetX)
		{
			float targetY = ((targetX - x1) * (y2 - y1) / (x2 - x1)) + y1;
			return targetY;
		}

		public static float Clamp(float x, float min, float max)
		{
			return (x < min) ? min : (x > max) ? max : x;
		}

		public static int GetNextPowerOf2(int v)
		{
			v--;
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v++;
			return v;
		}

		public static bool IsPowerOf2(int x)
		{
			return ((x & (x - 1)) == 0);
		}

	}
}

