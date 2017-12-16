using System.ComponentModel;
using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    [DoNotNotify]
    public class AGSImageComponent : AGSComponent, IImageComponent
    {
        private IHasImage _image;
        private IGraphicsFactory _factory;
        private IAnimationComponent _animationComponent;
        private IScaleComponent _scale;

        public AGSImageComponent(IHasImage image, IGraphicsFactory factory)
        {
            _image = image;
            _factory = factory;
            _image.PropertyChanged += onPropertyChanged;
        }

        [Property(Category = "Transform")]
        [NumberEditorSlider(sliderMin: 0, sliderMax: 1f)]
        public PointF Anchor { get => _image.Anchor; set => _image.Anchor = value; }

        public IImageRenderer CustomRenderer { get => _image.CustomRenderer; set => _image.CustomRenderer = value; }

        public IImage Image
        {
            get
            {
                return _animationComponent?.Animation?.Sprite?.Image;
            }
            set
            {
                AGSSingleFrameAnimation animation = new AGSSingleFrameAnimation(value, _factory);
                if (value != null)
                {
                    var scale = _scale;
                    if (scale != null) scale.BaseSize = new SizeF(value.Width, value.Height);
                }
                _animationComponent?.StartAnimation(animation);
            }
        }

        public byte Opacity { get => _image.Opacity; set => _image.Opacity = value; }

        public Color Tint { get => _image.Tint; set => _image.Tint = value; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<IAnimationComponent>(c => _animationComponent = c, _ => _animationComponent = null);
            entity.Bind<IScaleComponent>(c => _scale = c, _ => _scale = null);
        }

        private void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }
    }
}
