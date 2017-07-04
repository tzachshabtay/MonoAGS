using AGS.API;

namespace AGS.Engine
{
    public class AGSHasImage : IHasImage
    {
        private IImage _image;
        private PointF _anchor;
        private Color _tint;
        private object _imageArgs, _anchorArgs, _tintArgs;

        public AGSHasImage()
        {
            OnImageChanged = new AGSEvent<object>();
            OnAnchorChanged = new AGSEvent<object>();
            OnTintChanged = new AGSEvent<object>();
            _imageArgs = new object();
            _anchorArgs = new object();
            _tintArgs = new object();
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
                OnTintChanged.FireEvent(_tintArgs);
            }
        }

        public PointF Anchor 
        { 
            get { return _anchor; } 
            set 
            { 
                _anchor = value; 
                OnAnchorChanged.FireEvent(_anchorArgs); 
            } 
        }

        public IImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnImageChanged.FireEvent(_imageArgs);
            }
        }

        public IImageRenderer CustomRenderer { get; set; }

        public IEvent<object> OnImageChanged { get; private set; }

        public IEvent<object> OnAnchorChanged { get; private set; }

        public IEvent<object> OnTintChanged { get; private set; }

        public override string ToString()
        {
            return _image == null ? base.ToString() : _image.ToString();
        }
    }
}
