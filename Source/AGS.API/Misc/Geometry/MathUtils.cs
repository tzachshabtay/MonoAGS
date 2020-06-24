﻿using System;
using System.Diagnostics;
using System.Threading;

namespace AGS.API
{
	public static class MathUtils
	{
		private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random());

        /// <summary>
        /// Performs a linear interpolation between values (https://en.wikipedia.org/wiki/Linear_interpolation).
        /// </summary>
        /// <returns>The interpolated value.</returns>
        /// <param name="x1">The first x value.</param>
        /// <param name="y1">The first y value.</param>
        /// <param name="x2">The second x value.</param>
        /// <param name="y2">The second y value.</param>
        /// <param name="targetX">Target x.</param>
		public static float Lerp(float x1, float y1, float x2, float y2, float targetX)
		{
			float targetY = ((targetX - x1) * (y2 - y1) / (x2 - x1)) + y1;
            Trace.Assert(!float.IsNaN(targetY), $"Can't lerp when x1 ({x1}) is equal to x2 ({x2})");
			return targetY;
		}

        /// <summary>
        /// Clamps "x" to be between min and max.
        /// </summary>
        /// <returns>The clamp.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
		public static float Clamp(float x, float min, float max)
		{
			return (x < min) ? min : (x > max) ? max : x;
		}

        /// <summary>
        /// Gets the next number after "v" which is a power of 2.
        /// </summary>
        /// <returns>The next power of 2.</returns>
        /// <param name="v">The value.</param>
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

        /// <summary>
        /// Is "x" the power of 2?
        /// </summary>
        /// <returns><c>true</c>, if x is the power of 2, <c>false</c> otherwise.</returns>
        /// <param name="x">The x coordinate.</param>
		public static bool IsPowerOf2(int x)
		{
			return ((x & (x - 1)) == 0);
		}

        /// <summary>
        /// Returns a thread local random number generator.
        /// </summary>
        /// <returns>The random number generator.</returns>
		public static Random Random()
		{
			return _random.Value;
		}

        /// <summary>
        /// Returns a minimum between multiple values.
        /// </summary>
        /// <returns>The minimum.</returns>
        /// <param name="values">Values.</param>
		public static float Min(params float[] values)
		{
			float minValue = float.MaxValue;
			foreach (var value in values)
			{
				if (value < minValue) minValue = value;
			}
			return minValue;
		}

        /// <summary>
        /// Returns the maximum between multiple values.
        /// </summary>
        /// <returns>The max.</returns>
        /// <param name="values">Values.</param>
		public static float Max(params float[] values)
		{
			float maxValue = float.MinValue;
			foreach (var value in values)
			{
				if (value > maxValue) maxValue = value;
			}
			return maxValue;
		}

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <returns>The radians.</returns>
        /// <param name="degrees">Degrees.</param>
        public static float DegreesToRadians(float degrees)
        {
            const float factor = (float)(Math.PI / 180d);
            return degrees * factor;
        }

        /// <summary>
        /// Compare two floating numbers (with tolerance, as this is the only sensible way to compare floats: http://floating-point-gui.de/).
        /// </summary>
        /// <returns><c>true</c>, if equals was floated, <c>false</c> otherwise.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public static unsafe bool FloatEquals(float x, float y)
        {
            if (float.IsNaN(x) && float.IsNaN(y)) return true;
            return Math.Abs(x - y) < 0.0001f;
        }

	    /// <summary>
	    /// Returns the distance between 2 points.
	    /// </summary>
	    /// <param name="p1">First point</param>
	    /// <param name="p2">Second point</param>
	    /// <returns>The distance between the points</returns>
	    public static float Distance(PointF p1, PointF p2) => (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
	}
}
