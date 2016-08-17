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

		public GLColor(float r, float g, float b, float a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		#region IGLColor implementation

		public float R { get; private set; }

		public float G { get; private set; }

		public float B { get; private set; }

		public float A { get; private set; }

		#endregion

		#region IGLColorBuilder implementation

		public IGLColor Build(params IHasImage[] sprites)
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

		public override string ToString()
		{
			return string.Format("[GLColor: R={0}, G={1}, B={2}, A={3}]", R, G, B, A);
		}

		#endregion

		private float multiply(Func<IHasImage, float> getter, params IHasImage[] sprites)
		{
			return process ((arg1, arg2) => arg1 * arg2, getter, sprites);
		}

		private float process(Func<float, float, float> apply, Func<IHasImage, float> getter, params IHasImage[] sprites)
		{
			float result = float.NaN;
			bool firstIteration = true;
			foreach (IHasImage sprite in sprites) 
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

