using AGS.API;

namespace AGS.Engine
{
	public class AGSSprite : AGSComponent, ISprite
	{
		private readonly IHasImage _hasImage;
        private readonly ITransform _transform;        
        private readonly IRotate _rotate;
        private readonly IMaskLoader _maskLoader;

		public AGSSprite (IMaskLoader maskLoader)
		{
            _maskLoader = maskLoader;

            //todo: abstract it to the constructor
            _transform = new AGSTransform();
            _hasImage = new AGSHasImage();
            _hasImage.Anchor = new PointF();                        
            _rotate = new AGSRotate();

            ScaleX = 1;
            ScaleY = 1;
            _hasImage.OnImageChanged.Subscribe((sender, args) => ScaleBy(ScaleX, ScaleY));
        }

        private AGSSprite(AGSSprite sprite) : this(sprite._maskLoader)
        {
            _transform.Location = sprite._transform.Location;
            _hasImage.Anchor = sprite._hasImage.Anchor;
            _hasImage.Image = sprite._hasImage.Image;
            _hasImage.Tint = sprite._hasImage.Tint;
            _hasImage.CustomRenderer = sprite._hasImage.CustomRenderer;
            _rotate.Angle = sprite._rotate.Angle;
            ScaleX = sprite.ScaleX;
            ScaleY = sprite.ScaleY;
            Width = sprite.Width;
            Height = sprite.Height;
        }

        #region ISprite implementation

        public void ResetBaseSize(float initialWidth, float initialHeight)
        {
        }

        public void ResetScale(float initialWidth, float initialHeight)
        {
        }

        public void ResetScale()
        {
            ScaleX = 1;
            ScaleY = 1;
            var image = _hasImage.Image;
            if (image != null)
            {
                Width = _hasImage.Image.Width;
                Height = _hasImage.Image.Height;
            }
        }

        public void ScaleBy(float scaleX, float scaleY)
        {
            ScaleX = scaleX;
            ScaleY = scaleY;
            var image = _hasImage.Image;
            if (image != null)
            {
                Width = _hasImage.Image.Width * ScaleX;
                Height = _hasImage.Image.Height * ScaleY;
            }
        }

        public void ScaleTo(float width, float height)
        {
            Width = width;
            Height = height;
            var image = _hasImage.Image;
            if (image != null)
            {
                ScaleX = Width / _hasImage.Image.Width;
                ScaleY = Height / _hasImage.Image.Height;
            }
        }

        public void FlipHorizontally()
        {
            ScaleBy(-ScaleX, ScaleY);
            _hasImage.Anchor = new PointF(-_hasImage.Anchor.X, _hasImage.Anchor.Y);
        }

        public void FlipVertically()
        {
            ScaleBy(ScaleX, -ScaleY);
            _hasImage.Anchor = new PointF(_hasImage.Anchor.X, -_hasImage.Anchor.Y);
        }

        public ISprite Clone()
		{
            return new AGSSprite(this);
		}

        public ILocation Location { get { return _transform.Location; } set { _transform.Location = value; } }

        public float X { get { return _transform.X; } set { _transform.X = value; } }

        public float Y { get { return _transform.Y; } set { _transform.Y = value; } }

        public float Z { get { return _transform.Z; } set { _transform.Z = value; } }

        public float Height { get; private set; }

        public float Width { get; private set; }

        public float ScaleX { get; private set; }

        public float ScaleY { get; private set; }

        public float Angle { get { return _rotate.Angle; } set { _rotate.Angle = value; } }

        public PointF Anchor { get { return _hasImage.Anchor; } set { _hasImage.Anchor = value; } }

        public IImageRenderer CustomRenderer { get { return _hasImage.CustomRenderer; } set { _hasImage.CustomRenderer = value; } }

        public IImage Image { get { return _hasImage.Image; } set { _hasImage.Image = value; } }

        public IEvent<AGSEventArgs> OnImageChanged { get { return _hasImage.OnImageChanged; } }

        public byte Opacity { get { return _hasImage.Opacity; } set { _hasImage.Opacity = value; } }

        public Color Tint { get { return _hasImage.Tint; } set { _hasImage.Tint = value; } }

        public IArea PixelPerfectHitTestArea { get; private set; }
        public void PixelPerfect(bool pixelPerfect)
        {
            IArea area = PixelPerfectHitTestArea;
            if (!pixelPerfect)
            {
                if (area == null) return;
                area.Enabled = false;
                return;
            }
            if (area != null)
            {
                area.Enabled = true;
                return;
            }

            PixelPerfectHitTestArea = new AGSArea { Mask = _maskLoader.Load(_hasImage.Image.OriginalBitmap) };
        }
        #endregion

        public override string ToString()
		{
            return _hasImage.ToString();
		}
	}
}

