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
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
                if (BaseSize.Width == 0f && _image.Image != null) BaseSize = new SizeF(_image.Image.Width, _image.Image.Height);
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
            });
        }

        public float Height { get; private set; }

        public float Width { get; private set; }

        public float ScaleX 
        { 
            get { return _scaleX; } 
            set 
            {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
                if (_scaleX == value) return;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
                ScaleBy(value, ScaleY); 
            } 
        }

        public float ScaleY
        {
            get { return _scaleY; }
            set
            {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
                if (_scaleY == value) return;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
                ScaleBy(ScaleX, value);
            }
        }

        public SizeF BaseSize
        {
            get { return _baseSize; }
            set
            {
//#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator                
//                if (value.Width == _baseSize.Width && value.Height == _baseSize.Height) return;
//#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
                float width = value.Width * ScaleX;
                float height = value.Height * ScaleY;
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
                if (width == Width && height == Height) return;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
                Width = width;
                Height = height;
                _baseSize = new SizeF(value.Width, value.Height);
                fireScaleChange();
            }
        }

        public IEvent OnScaleChanged { get; private set; }

        public void ResetScale()
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (Width == BaseSize.Width && Height == BaseSize.Height && ScaleX == 1f && ScaleY == 1f) return;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
            Width = BaseSize.Width;
            Height = BaseSize.Height;
            _scaleX = 1f;
            _scaleY = 1f;
            fireScaleChange();
        }

        public void ResetScale(float initialWidth, float initialHeight)
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (BaseSize.Width == initialWidth && BaseSize.Height == initialHeight && ScaleX == 1f && ScaleY == 1f) return;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
            _baseSize = new SizeF(initialWidth, initialHeight);
            ResetScale();
        }

        public void ScaleBy(float scaleX, float scaleY)
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (ScaleX == scaleX && ScaleY == scaleY) return;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
            validateScaleInitialized();
            _scaleX = scaleX;
            _scaleY = scaleY;
            Width = BaseSize.Width * ScaleX;
            Height = BaseSize.Height * ScaleY;
            fireScaleChange();
        }

        public void ScaleTo(float width, float height)
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (Width == width && Height == height) return;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
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
            _image.Anchor = new PointF(-_image.Anchor.X, _image.Anchor.Y);
            fireScaleChange();
        }

        public void FlipVertically()
        {
            ScaleBy(ScaleX, -ScaleY);
            _image.Anchor = new PointF(_image.Anchor.X, -_image.Anchor.Y);
            fireScaleChange();
        }        

        private void validateScaleInitialized()
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (BaseSize.Width == 0f)
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
            {
                throw new InvalidOperationException(
                    "Initial size was not set. Either assign an animation/image to the object, use ResetBaseSize or use the appropriate constructor.");
            }
        }

        private void fireScaleChange()
        {
            OnScaleChanged.Invoke();
        }
    }
}
