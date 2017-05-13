using AGS.API;

namespace AGS.Engine
{
    public class AGSImageComponent : AGSComponent, IImageComponent
    {
        private IHasImage _image;
        private IGraphicsFactory _factory;
        private IAnimationContainer _animationContainer;
        private IScaleComponent _scale;

        public AGSImageComponent(IHasImage image, IGraphicsFactory factory)
        {
            _image = image;
            _factory = factory;
        }

        public PointF Anchor {  get { return _image.Anchor; } set { _image.Anchor = value; } }

        public IImageRenderer CustomRenderer { get { return _image.CustomRenderer; } set { _image.CustomRenderer = value; } }

        public IImage Image
        {
            get
            {
                if (_animationContainer.Animation == null || _animationContainer.Animation.Sprite == null) return null;
                return _animationContainer.Animation.Sprite.Image;
            }
            set
            {
                AGSSingleFrameAnimation animation = new AGSSingleFrameAnimation(value, _factory);
                if (value != null)
                {
                    _scale.ResetBaseSize(value.Width, value.Height);
                }
                _animationContainer.StartAnimation(animation);
            }
        }

        public IEvent<AGSEventArgs> OnImageChanged { get { return _image.OnImageChanged; } }

        public IEvent<AGSEventArgs> OnAnchorChanged { get { return _image.OnAnchorChanged; } }

        public IEvent<AGSEventArgs> OnTintChanged { get { return _image.OnTintChanged; } }

        public byte Opacity { get { return _image.Opacity; } set { _image.Opacity = value; } }

        public Color Tint { get { return _image.Tint; } set { _image.Tint = value; } }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _animationContainer = entity.GetComponent<IAnimationContainer>();
            _scale = entity.GetComponent<IScaleComponent>();
        }
    }
}
