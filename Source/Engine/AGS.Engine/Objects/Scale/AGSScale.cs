using AGS.API;

namespace AGS.Engine
{
    public class AGSScale : IScale
    {
        private IHasImage _image;

        public AGSScale(IHasImage image)
        {
            _image = image;

            ScaleX = 1;
            ScaleY = 1;
            _image.OnImageChanged.Subscribe((sender, args) => ScaleBy(ScaleX, ScaleY));
        }

        public float Height { get; private set; }

        public float Width { get; private set; }

        public float ScaleX { get; private set; }

        public float ScaleY { get; private set; }

        public void ResetScale()
        {
            ScaleX = 1;
            ScaleY = 1;
            var image = _image.Image;
            if (image != null)
            {
                Width = _image.Image.Width;
                Height = _image.Image.Height;
            }
        }

        public void ScaleBy(float scaleX, float scaleY)
        {
            ScaleX = scaleX;
            ScaleY = scaleY;
            var image = _image.Image;
            if (image != null)
            {
                Width = _image.Image.Width * ScaleX;
                Height = _image.Image.Height * ScaleY;
            }
        }

        public void ScaleTo(float width, float height)
        {
            Width = width;
            Height = height;
            var image = _image.Image;
            if (image != null)
            {
                ScaleX = Width / _image.Image.Width;
                ScaleY = Height / _image.Image.Height;
            }
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
    }
}
