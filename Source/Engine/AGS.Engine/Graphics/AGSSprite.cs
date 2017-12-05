using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    [PropertyFolder]
	public class AGSSprite : AGSComponent, ISprite
	{
		private readonly IHasImage _hasImage;
        private readonly ITranslate _translate;        
        private readonly IRotate _rotate;
        private readonly IMaskLoader _maskLoader;
        private readonly IScale _scale;
        private readonly Resolver _resolver;
        private readonly int _id;
        private static readonly IConcurrentHashSet<int> _registeredPixelPerfectIds = new AGSConcurrentHashSet<int>(1000);
        private static readonly SizeF _emptySize = new SizeF(1f, 1f);
        private static int _lastId = 0;

		public AGSSprite (Resolver resolver, IMaskLoader maskLoader)
		{
            _id = Interlocked.Increment(ref _lastId);
            _maskLoader = maskLoader;
            _resolver = resolver;

            //todo: abstract it to the constructor
            _translate = new AGSTranslate();
            _hasImage = new AGSHasImage();
            _hasImage.Anchor = new PointF();
            _scale = new AGSScale(_hasImage);
            _rotate = new AGSRotate();

            _scale.PropertyChanged += onScalePropertyChanged;
        }

        private AGSSprite(AGSSprite sprite) : this(sprite._resolver, sprite._maskLoader)
        {
            _translate.Location = sprite._translate.Location;
            _hasImage.Anchor = sprite._hasImage.Anchor;
            _hasImage.Image = sprite._hasImage.Image;
            _hasImage.Tint = sprite._hasImage.Tint;
            _hasImage.CustomRenderer = sprite._hasImage.CustomRenderer;
            _rotate.Angle = sprite._rotate.Angle;
            BaseSize = sprite.BaseSize;
            Scale = sprite.Scale;
        }

        #region ISprite implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void ResetScale(float initialWidth, float initialHeight)
        {
            _scale.ResetScale(initialWidth, initialHeight);
        }

        public void ResetScale()
        {
            _scale.ResetScale();
        }

        public void ScaleTo(float width, float height)
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
            return new AGSSprite(this);
		}

        public ILocation Location { get { return _translate.Location; } set { _translate.Location = value; } }

        public float X { get { return _translate.X; } set { _translate.X = value; } }

        public float Y { get { return _translate.Y; } set { _translate.Y = value; } }

        public float Z { get { return _translate.Z; } set { _translate.Z = value; } }

        [DoNotNotify]
        public float Height { get { return _scale.Height; } }

        [DoNotNotify]
        public float Width { get { return _scale.Width; } }

        [DoNotNotify]
        public float ScaleX { get { return _scale.ScaleX; } set { _scale.ScaleX = value; } }

        [DoNotNotify]
        public float ScaleY { get { return _scale.ScaleY; } set { _scale.ScaleY = value; }}

        [DoNotNotify]
        public PointF Scale { get { return _scale.Scale; } set { _scale.Scale = value; } }

        [DoNotNotify]
        public SizeF BaseSize { get { return _scale.BaseSize; } set { _scale.BaseSize = value; }}

        public float Angle { get { return _rotate.Angle; } set { _rotate.Angle = value; } }

        public PointF Anchor { get { return _hasImage.Anchor; } set { _hasImage.Anchor = value; } }

        public IImageRenderer CustomRenderer { get { return _hasImage.CustomRenderer; } set { _hasImage.CustomRenderer = value; } }

        public IImage Image { get { return _hasImage.Image; } set { _hasImage.Image = value; } }

        public IEvent OnImageChanged { get { return _hasImage.OnImageChanged; } }

        public byte Opacity { get { return _hasImage.Opacity; } set { _hasImage.Opacity = value; } }

        public Color Tint { get { return _hasImage.Tint; } set { _hasImage.Tint = value; } }

        public IEvent OnAnchorChanged { get { return _hasImage.OnAnchorChanged; } }
        public IEvent OnTintChanged { get { return _hasImage.OnTintChanged; } }

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

            string areaId = string.Format("Sprite_PixelPerfect_{0}_{1}", Image.ID, _id);
			if (!_registeredPixelPerfectIds.Add(_id))
			{
				//Without this check bitmap.LockBits can be called twice without unlocking in between which crashes GDI+: https://github.com/mono/libgdiplus/blob/0c0592d09c7393fc418fa7f65f54e8b3bcc14cf2/src/bitmap.c#L1960
				Debug.WriteLine("2 concurrent commands were given at the same time for setting pixel perfect areas, ignoring one to set pixel perfect as {0} for {1}", pixelPerfect, areaId);
				return;
			}

			string maskId = string.Format("Mask_{0}", areaId);
            PixelPerfectHitTestArea = new AGSArea(areaId, _resolver) { Mask = _maskLoader.Load(maskId, _hasImage.Image.OriginalBitmap) };
            var debugDraw = PixelPerfectHitTestArea.Mask.DebugDraw;
            if (debugDraw != null) debugDraw.RemoveComponent<IPixelPerfectComponent>(); //Removing the pixel perfect from the debug draw mask, otherwise it disables the pixel perfect for the images which can be used by actual characters
            PixelPerfectHitTestArea.Enabled = true;
        }
        #endregion

        public override string ToString()
		{
            return _hasImage.ToString();
		}

        public void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, args);
            }
        }

        private void onScalePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }
	}
}

