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
            _hasImage.Pivot = new PointF();
            _scale = new AGSScale(_hasImage);
            _rotate = new AGSRotate();

            _scale.PropertyChanged += onPropertyChanged;
            _hasImage.PropertyChanged += onPropertyChanged;
            _translate.PropertyChanged += onPropertyChanged;
        }

        private AGSSprite(AGSSprite sprite) : this(sprite._resolver, sprite._maskLoader)
        {
            _translate.Location = sprite._translate.Location;
            _hasImage.Pivot = sprite._hasImage.Pivot;
            _hasImage.Image = sprite._hasImage.Image;
            _hasImage.Tint = sprite._hasImage.Tint;
            _hasImage.CustomRenderer = sprite._hasImage.CustomRenderer;
            _rotate.Angle = sprite._rotate.Angle;
            BaseSize = sprite.BaseSize;
            Scale = sprite.Scale;
        }

        #region ISprite implementation

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

        public ISprite Clone() => new AGSSprite(this);

        [DoNotNotify]
        public ILocation Location { get => _translate.Location; set => _translate.Location = value; }

        [DoNotNotify]
        [Property(Browsable = false)]
        public float X { get => _translate.X; set => _translate.X = value; }

        [DoNotNotify]
        [Property(Browsable = false)]
        public float Y { get => _translate.Y; set => _translate.Y = value; }

        [DoNotNotify]
        [Property(Browsable = false)]
        public float Z { get => _translate.Z; set => _translate.Z = value; }

        [DoNotNotify]
        public float Height => _scale.Height;

        [DoNotNotify]
        public float Width => _scale.Width;

        [DoNotNotify]
        [Property(Browsable = false)]
        public float ScaleX { get => _scale.ScaleX; set => _scale.ScaleX = value; }

        [DoNotNotify]
        [Property(Browsable = false)]
        public float ScaleY { get => _scale.ScaleY; set => _scale.ScaleY = value; }

        [DoNotNotify]
        [NumberEditorSlider(sliderMin: 0f, sliderMax: 2f)]
        public PointF Scale { get => _scale.Scale; set => _scale.Scale = value; }

        [DoNotNotify]
        public SizeF BaseSize { get => _scale.BaseSize; set => _scale.BaseSize = value; }

        [NumberEditorSlider(sliderMin: -180f, sliderMax: 180f)]
        public float Angle { get => _rotate.Angle; set => _rotate.Angle = value; }

        [NumberEditorSlider(sliderMin: 0, sliderMax: 1f)]
        public PointF Pivot { get => _hasImage.Pivot; set => _hasImage.Pivot = value; }

        [DoNotNotify]
        public IImageRenderer CustomRenderer { get => _hasImage.CustomRenderer; set => _hasImage.CustomRenderer = value; }

        [DoNotNotify]
        public IImage Image { get => _hasImage.Image; set => _hasImage.Image = value; }

        [DoNotNotify]
        public byte Opacity { get => _hasImage.Opacity; set => _hasImage.Opacity = value; }

        [DoNotNotify]
        public Color Tint { get => _hasImage.Tint; set => _hasImage.Tint = value; }

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

            string areaId = $"Sprite_PixelPerfect_{Image.ID}_{_id}";
			if (!_registeredPixelPerfectIds.Add(_id))
			{
				//Without this check bitmap.LockBits can be called twice without unlocking in between which crashes GDI+: https://github.com/mono/libgdiplus/blob/0c0592d09c7393fc418fa7f65f54e8b3bcc14cf2/src/bitmap.c#L1960
				Debug.WriteLine("2 concurrent commands were given at the same time for setting pixel perfect areas, ignoring one to set pixel perfect as {0} for {1}", pixelPerfect, areaId);
				return;
			}

			string maskId = $"Mask_{areaId}";
            PixelPerfectHitTestArea = new AGSArea(areaId, _resolver) { Mask = _maskLoader.Load(maskId, _hasImage.Image.OriginalBitmap) };
            PixelPerfectHitTestArea.Mask.DebugDraw?.RemoveComponent<IPixelPerfectComponent>(); //Removing the pixel perfect from the debug draw mask, otherwise it disables the pixel perfect for the images which can be used by actual characters
            PixelPerfectHitTestArea.Enabled = true;
        }
        #endregion

        public override string ToString() => _hasImage.ToString();

        private void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }
	}
}

