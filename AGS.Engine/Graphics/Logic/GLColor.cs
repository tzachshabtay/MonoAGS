using System;
using AGS.API;


namespace AGS.Engine
{
	public class GLColor : IGLColor, IGLColorBuilder
	{
		private const float COLOR_FACTOR = 255f;

		public GLColor()
		{
		}

		#region IGLColor implementation

		public float R { get; private set; }

		public float G { get; private set; }

		public float B { get; private set; }

		public float A { get; private set; }

		#endregion

		#region IGLColorBuilder implementation

		public IGLColor Build(params ISprite[] sprites)
		{
			R = multiply (s => s.Tint.R / COLOR_FACTOR, sprites);
			G = multiply (s => s.Tint.G / COLOR_FACTOR, sprites);
			B = multiply (s => s.Tint.B / COLOR_FACTOR, sprites);
			A = multiply (s => s.Opacity / COLOR_FACTOR, sprites);
			return this;
		}

		public IGLColor Build(Color color)
		{
			R = color.R / COLOR_FACTOR;
			G = color.G / COLOR_FACTOR;
			B = color.B / COLOR_FACTOR;
			A = color.A / COLOR_FACTOR;
			return this;
		}

		#endregion

		private float multiply(Func<ISprite, float> getter, params ISprite[] sprites)
		{
			return process ((arg1, arg2) => arg1 * arg2, getter, sprites);
		}

		private float process(Func<float, float, float> apply, Func<ISprite, float> getter, params ISprite[] sprites)
		{
			float result = float.NaN;
			bool firstIteration = true;
			foreach (ISprite sprite in sprites) 
			{
				if (sprite == null)
					continue;
				float prop = getter (sprite);
				if (firstIteration) 
				{
					result = prop;
					firstIteration = false;
				}
				else
					result = apply (result, prop);
			}
			return result;
		}
	}
}

