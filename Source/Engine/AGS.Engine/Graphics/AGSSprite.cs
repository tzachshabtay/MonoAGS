using AGS.API;

namespace AGS.Engine
{
	public class AGSSprite : ISprite
	{
		private IImage _image;
        private IHasImage _hasImage;
		private IMaskLoader _maskLoader;
        private ITransform _transform;

		public AGSSprite (IMaskLoader maskLoader)
		{
			_maskLoader = maskLoader;

            //todo: abstract it to the constructor
            _transform = new AGSTransform();
            _hasImage = new AGSHasImage();
            _hasImage.OnImageChanged.Subscribe((sender, args) => ScaleBy(ScaleX, ScaleY));

			ScaleX = 1;
			ScaleY = 1;
			Anchor = new PointF ();

			Tint =  Colors.White;
			Location = AGSLocation.Empty ();
		}

		#region ISprite implementation

		public void ResetScale()
		{
			ScaleX = 1;
			ScaleY = 1;
			var image = Image;
			if (image != null)
			{
				Width = Image.Width;
				Height = Image.Height;
			}
		}

		public void ScaleBy (float scaleX, float scaleY)
		{
			ScaleX = scaleX;
			ScaleY = scaleY;
			var image = Image;
			if (image != null)
			{
				Width = Image.Width * ScaleX;
				Height = Image.Height * ScaleY;
			}
		}

		public void ScaleTo (float width, float height)
		{
			Width = width;
			Height = height;
			var image = Image;
			if (image != null)
			{
				ScaleX = Width / Image.Width;
				ScaleY = Height / Image.Height;
			}
		}

		public void FlipHorizontally()
		{
			ScaleBy(-ScaleX, ScaleY);
			Anchor = new PointF (-Anchor.X, Anchor.Y);
		}

		public void FlipVertically()
		{
			ScaleBy(ScaleX, -ScaleY);
			Anchor = new PointF (Anchor.X, -Anchor.Y);
		}

		public ISprite Clone()
		{
			return (ISprite)MemberwiseClone();
		}

        public ILocation Location { get { return _transform.Location; } set { _transform.Location = value; } }

        public float X { get { return _transform.X; } set { _transform.X = value; } }

        public float Y { get { return _transform.Y; } set { _transform.Y = value; } }

        public float Z { get { return _transform.Z; } set { _transform.Z = value; } }

        public float Height { get; private set; }

		public float Width { get; private set; }

		public float ScaleX { get; private set; }

		public float ScaleY { get; private set; }

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
			return _image == null ? base.ToString() : _image.ToString();
		}
	}
}

