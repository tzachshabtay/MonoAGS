using AGS.API;

namespace AGS.Engine
{
	public class AGSSprite : ISprite
	{
		private IHasImage _hasImage;
		private IMaskLoader _maskLoader;
        private ITransform _transform;
        private IScale _scale;

		public AGSSprite (IMaskLoader maskLoader)
		{
			_maskLoader = maskLoader;

            //todo: abstract it to the constructor
            _transform = new AGSTransform();
            _hasImage = new AGSHasImage();
            _scale = new AGSScale(_hasImage);            
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

		public float Angle { get; set; }

        public PointF Anchor { get { return _hasImage.Anchor; } set { _hasImage.Anchor = value; } }

        public IImageRenderer CustomRenderer { get { return _hasImage.CustomRenderer; } set { _hasImage.CustomRenderer = value; } }

        public IImage Image { get { return _hasImage.Image; } set { _hasImage.Image = value; } }

        public IEvent<AGSEventArgs> OnImageChanged { get { return _hasImage.OnImageChanged; } }

        public byte Opacity { get { return _hasImage.Opacity; } set { _hasImage.Opacity = value; } }

        public Color Tint { get { return _hasImage.Tint; } set { _hasImage.Tint = value; } }

        public IArea PixelPerfectHitTestArea  { get; private set; }
		public void PixelPerfect(bool pixelPerfect)
		{
			IArea area = PixelPerfectHitTestArea;
			if (!pixelPerfect)
			{
				if (area == null) return;
				area.Enabled = false;
				return;
			}
			if (area != null) return;

			PixelPerfectHitTestArea = new AGSArea { Mask = _maskLoader.Load(Image.OriginalBitmap) };
		}
		#endregion

		public override string ToString()
		{
            return _hasImage.ToString();
		}
	}
}

