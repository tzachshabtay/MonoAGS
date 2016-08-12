using AGS.API;

namespace AGS.Engine
{
	public class AGSSprite : ISprite
	{
		private readonly IHasImage _hasImage;
        private readonly ITransform _transform;
        private readonly IScale _scale;
        private readonly IPixelPerfectCollidable _pixelPerfect;
        private readonly IRotate _rotate;

		public AGSSprite (IMaskLoader maskLoader)
		{
            //todo: abstract it to the constructor
            _transform = new AGSTransform();
            _hasImage = new AGSHasImage();
            _scale = new AGSScale(_hasImage);
            _pixelPerfect = new AGSPixelPerfectCollidable(_hasImage, maskLoader);
            _rotate = new AGSRotate();
		}

		#region ISprite implementation

		public void ResetScale()
		{
            _scale.ResetScale();
		}

		public void ScaleBy (float scaleX, float scaleY)
		{
            _scale.ScaleBy(scaleX, scaleY);
		}

		public void ScaleTo (float width, float height)
		{
            _scale.ScaleTo(width, height);                
		}

		public void FlipHorizontally()
		{
            _scale.FlipHorizontally();
		}

		public void FlipVertically()
		{
            _scale.FlipVertically();
		}

		public ISprite Clone()
		{
			return (ISprite)MemberwiseClone();
		}

        public ILocation Location { get { return _transform.Location; } set { _transform.Location = value; } }

        public float X { get { return _transform.X; } set { _transform.X = value; } }

        public float Y { get { return _transform.Y; } set { _transform.Y = value; } }

        public float Z { get { return _transform.Z; } set { _transform.Z = value; } }

        public float Height { get { return _scale.Height; } }

		public float Width { get { return _scale.Width; } }

		public float ScaleX { get { return _scale.ScaleX; } }

		public float ScaleY { get { return _scale.ScaleY; } }

		public float Angle { get { return _rotate.Angle; } set { _rotate.Angle = value; } }

        public PointF Anchor { get { return _hasImage.Anchor; } set { _hasImage.Anchor = value; } }

        public IImageRenderer CustomRenderer { get { return _hasImage.CustomRenderer; } set { _hasImage.CustomRenderer = value; } }

        public IImage Image { get { return _hasImage.Image; } set { _hasImage.Image = value; } }

        public IEvent<AGSEventArgs> OnImageChanged { get { return _hasImage.OnImageChanged; } }

        public byte Opacity { get { return _hasImage.Opacity; } set { _hasImage.Opacity = value; } }

        public Color Tint { get { return _hasImage.Tint; } set { _hasImage.Tint = value; } }

        public IArea PixelPerfectHitTestArea { get { return _pixelPerfect.PixelPerfectHitTestArea; } }
		public void PixelPerfect(bool pixelPerfect)
		{
            _pixelPerfect.PixelPerfect(pixelPerfect); //A pixel perfect line!
		}
		#endregion

		public override string ToString()
		{
            return _hasImage.ToString();
		}
	}
}

