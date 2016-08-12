using AGS.API;

namespace AGS.Engine
{
    public class AGSHasImage : IHasImage
    {
        private IImage _image;

        public AGSHasImage()
        {
            OnImageChanged = new AGSEvent<AGSEventArgs>();
            Anchor = new PointF();
            Tint = Colors.White;
        }

        public byte Opacity
        {
            get { return Tint.A; }
            set { Tint = Color.FromArgb(value, Tint.R, Tint.G, Tint.B); }
        }

        public Color Tint { get; set; }

        public PointF Anchor { get; set; }

        public IImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnImageChanged.Invoke(this, new AGSEventArgs());
            }
        }

        public IImageRenderer CustomRenderer { get; set; }

        public IEvent<AGSEventArgs> OnImageChanged { get; private set; }

        public override string ToString()
        {
            return _image == null ? base.ToString() : _image.ToString();
        }
    }
}
