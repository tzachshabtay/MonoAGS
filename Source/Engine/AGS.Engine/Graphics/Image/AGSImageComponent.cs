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
                var animationContainer = _animationContainer;
                if (animationContainer == null || animationContainer.Animation == null || 
                    animationContainer.Animation.Sprite == null) return null;
                return animationContainer.Animation.Sprite.Image;
            }
            set
            {
                AGSSingleFrameAnimation animation = new AGSSingleFrameAnimation(value, _factory);
                if (value != null)
                {
                    var scale = _scale;
                    if (scale != null) scale.ResetBaseSize(value.Width, value.Height);
                }
                var animationContainer = _animationContainer;
                if (animationContainer != null) animationContainer.StartAnimation(animation);
            }
        }

        public IEvent<object> OnImageChanged { get { return _image.OnImageChanged; } }

        public IEvent<object> OnAnchorChanged { get { return _image.OnAnchorChanged; } }

        public IEvent<object> OnTintChanged { get { return _image.OnTintChanged; } }

        public byte Opacity { get { return _image.Opacity; } set { _image.Opacity = value; } }

        public Color Tint { get { return _image.Tint; } set { _image.Tint = value; } }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<IAnimationContainer>(c => _animationContainer = c, _ => _animationContainer = null);
            entity.Bind<IScaleComponent>(c => _scale = c, _ => _scale = null);
        }
    }
}
