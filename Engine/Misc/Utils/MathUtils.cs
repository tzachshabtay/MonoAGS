using System;
using System.Threading;

namespace AGS.Engine
{
	public static class MathUtils
	{
		private static ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random());

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

		public static Random Random()
		{
			return _random.Value;
		}

		public static float Min(params float[] values)
		{
			float minValue = float.MaxValue;
			foreach (var value in values)
			{
				if (value < minValue) minValue = value;
			}
			return minValue;
		}

		public static float Max(params float[] values)
		{
			float maxValue = float.MinValue;
			foreach (var value in values)
			{
				if (value > maxValue) maxValue = value;
			}
			return maxValue;
		}
	}
}

