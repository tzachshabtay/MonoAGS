using System;
using AGS.API;


namespace AGS.Engine
{
    public struct GLColor : IEquatable<GLColor>
	{
		private const float COLOR_FACTOR = 255f;

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

		public GLColor Build(params IHasImage[] sprites)
		{
            float r = multiply(s => (s.Tint.R / COLOR_FACTOR) * s.Brightness.X, sprites);
            float g = multiply(s => (s.Tint.G / COLOR_FACTOR) * s.Brightness.Y, sprites);
            float b = multiply(s => (s.Tint.B / COLOR_FACTOR) * s.Brightness.Z, sprites);
            float a = multiply(s => (s.Opacity / COLOR_FACTOR) * s.Brightness.W, sprites);
            return new GLColor(r, g, b, a);
		}

		public GLColor Build(Color color)
		{
			float r = color.R / COLOR_FACTOR;
			float g = color.G / COLOR_FACTOR;
			float b = color.B / COLOR_FACTOR;
			float a = color.A / COLOR_FACTOR;
            return new GLColor(r, g, b, a);
		}

		public override string ToString()
		{
			return $"[GLColor: R={R}, G={G}, B={B}, A={A}]";
		}

		public override bool Equals(object obj)
		{
            return (obj is GLColor color) && Equals(color);
		}
        
        public bool Equals(GLColor other)
        {
            return MathUtils.FloatEquals(other.R, R) && MathUtils.FloatEquals(other.G, G) &&
                   MathUtils.FloatEquals(other.B, B) && MathUtils.FloatEquals(other.A, A);
        }

		public override int GetHashCode()
		{
            return R.GetHashCode(); //todo: this is a potential bug as R is not readonly
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
