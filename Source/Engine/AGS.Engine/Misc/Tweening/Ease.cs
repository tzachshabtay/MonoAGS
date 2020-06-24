﻿using System;

//Code taken from: https://github.com/eropple/UnglidePCL/blob/master/Unglide.Portable/Ease.cs
namespace AGS.Engine
{
	/// <summary>
	/// Static class with useful easer functions that can be used by Tweens.
	/// </summary>
	public static class Ease
	{
		const float Pi = 3.14159f;
		const float Pi2 = Pi / 2;
		const float B1 = 1 / 2.75f;
		const float B2 = 2 / 2.75f;
		const float B3 = 1.5f / 2.75f;
		const float B4 = 2.5f / 2.75f;
		const float B5 = 2.25f / 2.75f;
		const float B6 = 2.625f / 2.75f;

		/// <summary>
		/// Linear.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float Linear(float t)
		{
			return t;
		}

		/// <summary>
		/// Elastic in.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float ElasticIn(float t)
		{
			return (float)(Math.Sin(13 * Pi2 * t) * Math.Pow(2, 10 * (t - 1)));
		}

		/// <summary>
		/// Elastic out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float ElasticOut(float t)
		{
			return (float)(Math.Sin(-13 * Pi2 * (t + 1)) * Math.Pow(2, -10 * t) + 1);
		}

		/// <summary>
		/// Elastic in and out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float ElasticInOut(float t)
		{
			if (t < 0.5)
			{
				return (float)(0.5 * Math.Sin(13 * Pi2 * (2 * t)) * Math.Pow(2, 10 * ((2 * t) - 1)));
			}

			return (float)(0.5 * (Math.Sin(-13 * Pi2 * ((2 * t - 1) + 1)) * Math.Pow(2, -10 * (2 * t - 1)) + 2));
		}

		/// <summary>
		/// Quadratic in.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float QuadIn(float t)
		{
			return t * t;
		}

		/// <summary>
		/// Quadratic out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float QuadOut(float t)
		{
			return -t * (t - 2);
		}

		/// <summary>
		/// Quadratic in and out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float QuadInOut(float t)
		{
			return t <= .5 ? t * t * 2 : 1 - (--t) * t * 2;
		}

		/// <summary>
		/// Cubic in.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float CubeIn(float t)
		{
			return t * t * t;
		}

		/// <summary>
		/// Cubic out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float CubeOut(float t)
		{
			return 1 + (--t) * t * t;
		}

		/// <summary>
		/// Cubic in and out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float CubeInOut(float t)
		{
			return t <= .5 ? t * t * t * 4 : 1 + (--t) * t * t * 4;
		}

		/// <summary>
		/// Quart in.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float QuartIn(float t)
		{
			return t * t * t * t;
		}

		/// <summary>
		/// Quart out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float QuartOut(float t)
		{
			return 1 - (t -= 1) * t * t * t;
		}

		/// <summary>
		/// Quart in and out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float QuartInOut(float t)
		{
			return (float)(t <= .5 ? t * t * t * t * 8 : (1 - (t = t * 2 - 2) * t * t * t) / 2 + .5);
		}

		/// <summary>
		/// Quint in.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float QuintIn(float t)
		{
			return t * t * t * t * t;
		}

		/// <summary>
		/// Quint out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float QuintOut(float t)
		{
			return (t = t - 1) * t * t * t * t + 1;
		}

		/// <summary>
		/// Quint in and out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float QuintInOut(float t)
		{
			return ((t *= 2) < 1) ? (t * t * t * t * t) / 2 : ((t -= 2) * t * t * t * t + 2) / 2;
		}

		/// <summary>
		/// Sine in.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float SineIn(float t)
		{
			return (float)(-Math.Cos(Pi2 * t) + 1);
		}

		/// <summary>
		/// Sine out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float SineOut(float t)
		{
			return (float)(Math.Sin(Pi2 * t));
		}

		/// <summary>
		/// Sine in and out
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float SineInOut(float t)
		{
			return (float)(-Math.Cos(Pi * t) / 2 + .5);
		}

		/// <summary>
		/// Bounce in.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float BounceIn(float t)
		{
			t = 1 - t;
			if (t < B1) return (float)(1 - 7.5625 * t * t);
			if (t < B2) return (float)(1 - (7.5625 * (t - B3) * (t - B3) + .75));
			if (t < B4) return (float)(1 - (7.5625 * (t - B5) * (t - B5) + .9375));
			return (float)(1 - (7.5625 * (t - B6) * (t - B6) + .984375));
		}

		/// <summary>
		/// Bounce out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float BounceOut(float t)
		{
			if (t < B1) return (float)(7.5625 * t * t);
			if (t < B2) return (float)(7.5625 * (t - B3) * (t - B3) + .75);
			if (t < B4) return (float)(7.5625 * (t - B5) * (t - B5) + .9375);
			return (float)(7.5625 * (t - B6) * (t - B6) + .984375);
		}

		/// <summary>
		/// Bounce in and out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float BounceInOut(float t)
		{
			if (t < .5)
			{
				t = 1 - t * 2;
				if (t < B1) return (float)((1 - 7.5625 * t * t) / 2);
				if (t < B2) return (float)((1 - (7.5625 * (t - B3) * (t - B3) + .75)) / 2);
				if (t < B4) return (float)((1 - (7.5625 * (t - B5) * (t - B5) + .9375)) / 2);
				return (float)((1 - (7.5625 * (t - B6) * (t - B6) + .984375)) / 2);
			}
			t = t * 2 - 1;
			if (t < B1) return (float)((7.5625 * t * t) / 2 + .5);
			if (t < B2) return (float)((7.5625 * (t - B3) * (t - B3) + .75) / 2 + .5);
			if (t < B4) return (float)((7.5625 * (t - B5) * (t - B5) + .9375) / 2 + .5);
			return (float)((7.5625 * (t - B6) * (t - B6) + .984375) / 2 + .5);
		}

		/// <summary>
		/// Circle in.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float CircIn(float t)
		{
			return (float)(-(Math.Sqrt(1 - t * t) - 1));
		}

		/// <summary>
		/// Circle out
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float CircOut(float t)
		{
			return (float)(Math.Sqrt(1 - (t - 1) * (t - 1)));
		}

		/// <summary>
		/// Circle in and out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float CircInOut(float t)
		{
			return (float)(t <= .5 ? (Math.Sqrt(1 - t * t * 4) - 1) / -2 : (Math.Sqrt(1 - (t * 2 - 2) * (t * 2 - 2)) + 1) / 2);
		}

		/// <summary>
		/// Exponential in.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float ExpoIn(float t)
		{
			return (float)(Math.Pow(2, 10 * (t - 1)));
		}

		/// <summary>
		/// Exponential out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float ExpoOut(float t)
		{
			return (float)(-Math.Pow(2, -10 * t) + 1);
		}

		/// <summary>
		/// Exponential in and out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float ExpoInOut(float t)
		{
			return (float)(t < .5 ? Math.Pow(2, 10 * (t * 2 - 1)) / 2 : (-Math.Pow(2, -10 * (t * 2 - 1)) + 2) / 2);
		}

		/// <summary>
		/// Back in.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float BackIn(float t)
		{
			return (float)(t * t * (2.70158 * t - 1.70158));
		}

		/// <summary>
		/// Back out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float BackOut(float t)
		{
			return (float)(1 - (--t) * (t) * (-2.70158 * t - 1.70158));
		}

		/// <summary>
		/// Back in and out.
		/// </summary>
		/// <param name="t">Time elapsed.</param>
		/// <returns>Eased timescale.</returns>
		public static float BackInOut(float t)
		{
			t *= 2;
			if (t < 1) return (float)(t * t * (2.70158 * t - 1.70158) / 2);
			t--;
			return (float)((1 - (--t) * (t) * (-2.70158 * t - 1.70158)) / 2 + .5);
		}
	}
}

