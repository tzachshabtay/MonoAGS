using System.ComponentModel;
using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    [DoNotNotify]
    public class AGSImageComponent : AGSComponent, IImageComponent, ISpriteProvider
    {
        private IHasImage _image;
        private IGraphicsFactory _factory;
        private ISprite _sprite;
        private IScaleComponent _scale;
        private ISpriteProvider _provider;

        public AGSImageComponent(IHasImage image, IGraphicsFactory factory)
        {
            _image = image;
            _factory = factory;
            _image.PropertyChanged += onPropertyChanged;
        }

        [Property(Category = "Transform", CategoryZ = -100, CategoryExpand = true)]
        [NumberEditorSlider(sliderMin: 0, sliderMax: 1f)]
        public PointF Pivot { get => _image.Pivot; set => _image.Pivot = value; }

        public IImageRenderer CustomRenderer { get => _image.CustomRenderer; set => _image.CustomRenderer = value; }


        [DoNotNotify]
        public IImage Image
        {
            get
            {
                return _provider?.Sprite?.Image;
            }
            set
            {
                if (_sprite == null)
                {
                    _sprite = _factory.GetSprite();
                    _sprite.Location = AGSLocation.Empty();
                }
                _sprite.Image = value;
                
                if (value != null)
                {
                    var scale = _scale;
                    if (scale != null) scale.BaseSize = new SizeF(value.Width, value.Height);
                    SpriteProvider = this;
                }
            }
        }

        public byte Opacity { get => _image.Opacity; set => _image.Opacity = value; }

        public Color Tint { get => _image.Tint; set => _image.Tint = value; }

        [NumberEditorSlider(sliderMin: 0f, sliderMax: 2f, step: 0.1f)]
        public Vector4 Brightness { get => _image.Brightness; set => _image.Brightness = value; }

        public ISprite Sprite { get => _sprite; }

        public ISprite CurrentSprite { get => _provider?.Sprite; }

        public ISpriteProvider SpriteProvider
        {
            get => _provider;
            set
            {
                var previousProvider = _provider;
                if (previousProvider != null)
                    previousProvider.PropertyChanged -= OnProviderPropertyChanged;
                if (value != null)
                    value.PropertyChanged += OnProviderPropertyChanged;
                _provider = value;
                if (value != this)
                {
                    Image = null;
                }
                OnPropertyChanged(nameof(CurrentSprite));
                OnPropertyChanged(nameof(Image));
            }
        }

        public bool DebugDrawPivot { get; set; }

        public IBorderStyle Border { get; set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<IScaleComponent>(c => _scale = c, _ => _scale = null);
        }

        private void OnProviderPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(ISpriteProvider.Sprite))
                return;
            // resend property changed event to notify that ISpriteRenderComponent.CurrentSprite has new value
            OnPropertyChanged(nameof(CurrentSprite));
            OnPropertyChanged(nameof(Image));
        }

        private void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }
    }
}
