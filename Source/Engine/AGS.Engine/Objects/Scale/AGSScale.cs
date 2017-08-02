using AGS.API;
using System;

namespace AGS.Engine
{
    public class AGSScale : IScale
    {
        private IHasImage _image;

        public AGSScale(IHasImage image) : this(image, true)
        {
        }

        public AGSScale(IHasImage image, float width, float height) : this(image, false)
        {
            ResetBaseSize(width, height);
        }

        private AGSScale(IHasImage image, bool shouldSubscribeToImageChange)
        { 
            _image = image;
            OnScaleChanged = new AGSEvent<object>();

            ScaleX = 1;
            ScaleY = 1;

            if (!shouldSubscribeToImageChange) return;
            image.OnImageChanged.Subscribe(args =>
            {
                if (BaseSize.Width == 0f) ResetBaseSize(_image.Image.Width, _image.Image.Height);
            });
        }

        public float Height { get; private set; }

        public float Width { get; private set; }

        public float ScaleX { get; private set; }

        public float ScaleY { get; private set; }

        public SizeF BaseSize { get; private set; }

        public IEvent<object> OnScaleChanged { get; private set; }

        public void ResetBaseSize(float initialWidth, float initialHeight)
        {
            float width = initialWidth * ScaleX;
            float height = initialHeight * ScaleY;
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (width == Width && height == Height) return;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
            Width = width;
            Height = height;
            BaseSize = new SizeF(initialWidth, initialHeight);
            fireScaleChange();
        }

        public void ResetScale()
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (Width == BaseSize.Width && Height == BaseSize.Height && ScaleX == 1f && ScaleY == 1f) return;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
            Width = BaseSize.Width;
            Height = BaseSize.Height;
            ScaleX = 1f;
            ScaleY = 1f;
            fireScaleChange();
        }

        public void ResetScale(float initialWidth, float initialHeight)
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (BaseSize.Width == initialWidth && BaseSize.Height == initialHeight && ScaleX == 1f && ScaleY == 1f) return;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
            BaseSize = new SizeF(initialWidth, initialHeight);
            ResetScale();
        }

        public void ScaleBy(float scaleX, float scaleY)
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (ScaleX == scaleX && ScaleY == scaleY) return;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
            validateScaleInitialized();
            ScaleX = scaleX;
            ScaleY = scaleY;
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
            ScaleX = Width / BaseSize.Width;
            ScaleY = Height / BaseSize.Height;
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
            if (BaseSize.Width == 0f)
            {
                throw new InvalidOperationException(
                    "Initial size was not set. Either assign an animation/image to the object, use ResetBaseSize or use the appropriate constructor.");
            }
        }

        private void fireScaleChange()
        {
            OnScaleChanged.FireEvent(null);
        }
    }
}
