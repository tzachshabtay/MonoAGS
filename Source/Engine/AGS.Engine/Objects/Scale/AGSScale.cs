using AGS.API;
using System;

namespace AGS.Engine
{
    public class AGSScale : IScale
    {
        private IHasImage _image;
        private float _scaleX, _scaleY;
        private SizeF _baseSize;

        public AGSScale(IHasImage image) : this(image, true)
        {
        }

        public AGSScale(IHasImage image, float width, float height) : this(image, false)
        {
            BaseSize = new SizeF(width, height);
        }

        private AGSScale(IHasImage image, bool shouldSubscribeToImageChange)
        { 
            _image = image;
            OnScaleChanged = new AGSEvent();

            _scaleX = 1;
            _scaleY = 1;

            if (!shouldSubscribeToImageChange) return;
            image.OnImageChanged.Subscribe(() =>
            {
                if (MathUtils.FloatEquals(BaseSize.Width, 0f) && _image.Image != null) BaseSize = new SizeF(_image.Image.Width, _image.Image.Height);
            });
        }

        public float Height { get; private set; }

        public float Width { get; private set; }

        public PointF Scale 
        { 
            get { return new PointF(_scaleX, _scaleY); }
            set { ScaleBy(value.X, value.Y);}
        }

        [Property(Browsable = false)]
        public float ScaleX 
        { 
            get { return _scaleX; } 
            set 
            {
                if (MathUtils.FloatEquals(_scaleX, value)) return;
                ScaleBy(value, ScaleY); 
            } 
        }

        [Property(Browsable = false)]
        public float ScaleY
        {
            get { return _scaleY; }
            set
            {
                if (MathUtils.FloatEquals(_scaleY, value)) return;
                ScaleBy(ScaleX, value);
            }
        }

        public SizeF BaseSize
        {
            get { return _baseSize; }
            set
            {
                float width = value.Width * ScaleX;
                float height = value.Height * ScaleY;
                if (MathUtils.FloatEquals(width, Width) && MathUtils.FloatEquals(height, Height)) return;
                Width = width;
                Height = height;
                _baseSize = new SizeF(value.Width, value.Height);
                fireScaleChange();
            }
        }

        public IEvent OnScaleChanged { get; private set; }

        public void ResetScale()
        {
            if (MathUtils.FloatEquals(Width, BaseSize.Width) && MathUtils.FloatEquals(Height, BaseSize.Height) &&
                MathUtils.FloatEquals(ScaleX, 1f) && MathUtils.FloatEquals(ScaleY, 1f)) return;
            Width = BaseSize.Width;
            Height = BaseSize.Height;
            _scaleX = 1f;
            _scaleY = 1f;
            fireScaleChange();
        }

        public void ResetScale(float initialWidth, float initialHeight)
        {
            if (MathUtils.FloatEquals(BaseSize.Width, initialWidth) && MathUtils.FloatEquals(BaseSize.Height, initialHeight) && 
                MathUtils.FloatEquals(ScaleX, 1f) && MathUtils.FloatEquals(ScaleY, 1f)) return;
            _baseSize = new SizeF(initialWidth, initialHeight);
            ResetScale();
        }

        public void ScaleBy(float scaleX, float scaleY)
        {
            if (MathUtils.FloatEquals(ScaleX, scaleX) && MathUtils.FloatEquals(ScaleY, scaleY)) return;
            validateScaleInitialized();
            _scaleX = scaleX;
            _scaleY = scaleY;
            Width = BaseSize.Width * ScaleX;
            Height = BaseSize.Height * ScaleY;
            fireScaleChange();
        }

        public void ScaleTo(float width, float height)
        {
            if (MathUtils.FloatEquals(Width, width) && MathUtils.FloatEquals(Height, height)) return;
            validateScaleInitialized();
            Width = width;
            Height = height;
            _scaleX = Width / BaseSize.Width;
            _scaleY = Height / BaseSize.Height;
            fireScaleChange();
        }

        public void FlipHorizontally()
        {
            ScaleBy(-ScaleX, ScaleY);
        }

        public void FlipVertically()
        {
            ScaleBy(ScaleX, -ScaleY);
        }        

        private void validateScaleInitialized()
        {
            if (MathUtils.FloatEquals(BaseSize.Width, 0f))
            {
                throw new InvalidOperationException(
                    "Initial size was not set. Either assign an animation/image to the object, or set BaseSize.");
            }
        }

        private void fireScaleChange()
        {
            OnScaleChanged.Invoke();
        }
    }
}
