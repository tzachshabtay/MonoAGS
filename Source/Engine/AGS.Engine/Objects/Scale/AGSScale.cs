using AGS.API;
using System;

namespace AGS.Engine
{
    public class AGSScale : IScale
    {
        private IHasImage _image;
        private float _initialWidth, _initialHeight;

        public AGSScale(IHasImage image)
        {
            _image = image;

            ScaleX = 1;
            ScaleY = 1;
            image.OnImageChanged.Subscribe((sender, args) =>
            {
                if (_initialWidth == 0f) ResetBaseSize(_image.Image.Width, _image.Image.Height);
            });
        }

        public AGSScale(IHasImage image, float width, float height)
        {
            _image = image;

            ScaleX = 1;
            ScaleY = 1;
            ResetBaseSize(width, height);
        }

        public float Height { get; private set; }

        public float Width { get; private set; }

        public float ScaleX { get; private set; }

        public float ScaleY { get; private set; }

        public void ResetBaseSize(float initialWidth, float initialHeight)
        {
            Width = initialWidth * ScaleX;
            Height = initialHeight * ScaleY;
            _initialWidth = initialWidth;
            _initialHeight = initialHeight;
        }

        public void ResetScale()
        {
            Width = _initialWidth;
            Height = _initialHeight;
            ScaleX = 1;
            ScaleY = 1;
        }

        public void ResetScale(float initialWidth, float initialHeight)
        {
            _initialWidth = initialWidth;
            _initialHeight = initialHeight;
            ResetScale();
        }

        public void ScaleBy(float scaleX, float scaleY)
        {
            validateScaleInitialized();
            ScaleX = scaleX;
            ScaleY = scaleY;
            Width = _initialWidth * ScaleX;
            Height = _initialHeight * ScaleY;
        }

        public void ScaleTo(float width, float height)
        {
            validateScaleInitialized();
            Width = width;
            Height = height;
            ScaleX = Width / _initialWidth;
            ScaleY = Height / _initialHeight;
        }

        public void FlipHorizontally()
        {
            ScaleBy(-ScaleX, ScaleY);
            _image.Anchor = new PointF(-_image.Anchor.X, _image.Anchor.Y);
        }

        public void FlipVertically()
        {
            ScaleBy(ScaleX, -ScaleY);
            _image.Anchor = new PointF(_image.Anchor.X, -_image.Anchor.Y);
        }        

        private void validateScaleInitialized()
        {
            if (_initialWidth == 0f)
            {
                throw new InvalidOperationException(
                    "Initial size was not set. Either assign an animation/image to the object, or use the appropriate constructor.");
            }
        }
    }
}
