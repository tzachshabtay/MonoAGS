using AGS.API;
using System;

namespace AGS.Engine
{
    public class AGSScale : IScale
    {
        private IHasImage _image;
        private readonly AGSEventArgs _args = new AGSEventArgs();

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
            OnScaleChanged = new AGSEvent<AGSEventArgs>();

            ScaleX = 1;
            ScaleY = 1;

            if (!shouldSubscribeToImageChange) return;
            image.OnImageChanged.Subscribe((sender, args) =>
            {
                if (BaseSize.Width == 0f) ResetBaseSize(_image.Image.Width, _image.Image.Height);
            });
        }

        public float Height { get; private set; }

        public float Width { get; private set; }

        public float ScaleX { get; private set; }

        public float ScaleY { get; private set; }

        public SizeF BaseSize { get; private set; }

        public IEvent<AGSEventArgs> OnScaleChanged { get; private set; }

        public void ResetBaseSize(float initialWidth, float initialHeight)
        {
            Width = initialWidth * ScaleX;
            Height = initialHeight * ScaleY;
            BaseSize = new SizeF(initialWidth, initialHeight);
            fireScaleChange();
        }

        public void ResetScale()
        {
            Width = BaseSize.Width;
            Height = BaseSize.Height;
            ScaleX = 1;
            ScaleY = 1;
            fireScaleChange();
        }

        public void ResetScale(float initialWidth, float initialHeight)
        {
            BaseSize = new SizeF(initialWidth, initialHeight);
            ResetScale();
        }

        public void ScaleBy(float scaleX, float scaleY)
        {
            validateScaleInitialized();
            ScaleX = scaleX;
            ScaleY = scaleY;
            Width = BaseSize.Width * ScaleX;
            Height = BaseSize.Height * ScaleY;
            fireScaleChange();
        }

        public void ScaleTo(float width, float height)
        {
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
            OnScaleChanged.FireEvent(this, _args);
        }
    }
}
