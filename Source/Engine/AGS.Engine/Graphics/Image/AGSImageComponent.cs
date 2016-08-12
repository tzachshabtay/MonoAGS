using AGS.API;

namespace AGS.Engine
{
    public class AGSImageComponent : AGSComponent, IImageComponent
    {
        private IHasImage _image;

        public AGSImageComponent(IHasImage image)
        {
            _image = image;
        }

        public PointF Anchor {  get { return _image.Anchor; } set { _image.Anchor = value; } }

        public IImageRenderer CustomRenderer { get { return _image.CustomRenderer; } set { _image.CustomRenderer = value; } }

        public IImage Image { get { return _image.Image; } set { _image.Image = value; } }

        public IEvent<AGSEventArgs> OnImageChanged { get { return _image.OnImageChanged; } }

        public byte Opacity { get { return _image.Opacity; } set { _image.Opacity = value; } }

        public Color Tint { get { return _image.Tint; } set { _image.Tint = value; } }
    }
}
