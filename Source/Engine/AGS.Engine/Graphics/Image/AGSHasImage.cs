using AGS.API;

namespace AGS.Engine
{
    public class AGSHasImage : IHasImage
    {
        private IImage _image;
        private PointF _anchor;
        private Color _tint;
        private AGSEventArgs _imageArgs, _anchorArgs, _tintArgs;

        public AGSHasImage()
        {
            OnImageChanged = new AGSEvent<AGSEventArgs>();
            OnAnchorChanged = new AGSEvent<AGSEventArgs>();
            OnTintChanged = new AGSEvent<AGSEventArgs>();
            _imageArgs = new AGSEventArgs();
            _anchorArgs = new AGSEventArgs();
            _tintArgs = new AGSEventArgs();
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
                OnTintChanged.FireEvent(this, _tintArgs);
            }
        }

        public PointF Anchor 
        { 
            get { return _anchor; } 
            set 
            { 
                _anchor = value; 
                OnAnchorChanged.FireEvent(this, _anchorArgs); 
            } 
        }

        public IImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnImageChanged.FireEvent(this, _imageArgs);
            }
        }

        public IImageRenderer CustomRenderer { get; set; }

        public IEvent<AGSEventArgs> OnImageChanged { get; private set; }

        public IEvent<AGSEventArgs> OnAnchorChanged { get; private set; }

        public IEvent<AGSEventArgs> OnTintChanged { get; private set; }

        public override string ToString()
        {
            return _image == null ? base.ToString() : _image.ToString();
        }
    }
}
