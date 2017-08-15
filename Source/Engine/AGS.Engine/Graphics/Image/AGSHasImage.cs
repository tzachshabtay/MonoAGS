using AGS.API;

namespace AGS.Engine
{
    public class AGSHasImage : IHasImage
    {
        private IImage _image;
        private PointF _anchor;
        private Color _tint;

        public AGSHasImage()
        {
            OnImageChanged = new AGSEvent();
            OnAnchorChanged = new AGSEvent();
            OnTintChanged = new AGSEvent();
            Anchor = new PointF(0.5f, 0f);
            Tint = Colors.White;
        }

        public byte Opacity
        {
            get { return Tint.A; }
            set { Tint = Color.FromArgb(value, Tint.R, Tint.G, Tint.B); }
        }

        public Color Tint 
        { 
            get { return _tint; }
            set
            {
                _tint = value;
                OnTintChanged.Invoke();
            }
        }

        public PointF Anchor 
        { 
            get { return _anchor; } 
            set 
            { 
                _anchor = value; 
                OnAnchorChanged.Invoke(); 
            } 
        }

        public IImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnImageChanged.Invoke();
            }
        }

        public IImageRenderer CustomRenderer { get; set; }

        public IEvent OnImageChanged { get; private set; }

        public IEvent OnAnchorChanged { get; private set; }

        public IEvent OnTintChanged { get; private set; }

        public override string ToString()
        {
            return _image == null ? base.ToString() : _image.ToString();
        }
    }
}
