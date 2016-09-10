using System;

namespace AGS.API
{
	public struct SizeF
	{
		private readonly float _width, _height;

		public SizeF(float width, float height)
		{
			_width = width;
			_height = height;
		}

		public float Width { get { return _width; } }
		public float Height { get { return _height; } }

        public override string ToString()
        {
            return string.Format("{0},{1}", Width, Height);
        }

        public SizeF Scale(float factorX, float factorY)
        {
            return new SizeF(_width * factorX, _height * factorY);
        }
    }
}

