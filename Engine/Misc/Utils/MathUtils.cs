using System;

namespace Engine
{
	public static class MathUtils
	{
		public static float Lerp(float x1, float y1, float x2, float y2, float targetX)
		{
			float targetY = ((targetX - x1) * (y2 - y1) / (x2 - x1)) + y1;
			return targetY;
		}
	}
}

