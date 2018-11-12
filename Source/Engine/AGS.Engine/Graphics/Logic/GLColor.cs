using System;
using AGS.API;


namespace AGS.Engine
{
    public class GLColor : IGLColor, IGLColorBuilder, IEquatable<GLColor>
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
            R = multiply(s => (s.Tint.R / COLOR_FACTOR) * s.Brightness.X, sprites);
            G = multiply(s => (s.Tint.G / COLOR_FACTOR) * s.Brightness.Y, sprites);
            B = multiply(s => (s.Tint.B / COLOR_FACTOR) * s.Brightness.Z, sprites);
            A = multiply(s => (s.Opacity / COLOR_FACTOR) * s.Brightness.W, sprites);
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
