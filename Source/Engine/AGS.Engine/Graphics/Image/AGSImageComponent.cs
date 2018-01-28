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
        private ISpriteRenderComponent _spriteRender;

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

        public IImage Image
        {
            get
            {
                return _sprite?.Image;
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
                    if (_spriteRender != null)
                        _spriteRender.SpriteProvider = this;
                }
            }
        }

        public byte Opacity { get => _image.Opacity; set => _image.Opacity = value; }

        public Color Tint { get => _image.Tint; set => _image.Tint = value; }

        public ISprite Sprite { get => _sprite; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<IScaleComponent>(c => _scale = c, _ => _scale = null);
            entity.Bind<ISpriteRenderComponent>(
                c => { _spriteRender = c; c.PropertyChanged += onSpriteRenderPropertyChanged; },
                c => { c.PropertyChanged -= onSpriteRenderPropertyChanged; _spriteRender = null; });
        }

        private void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        private void onSpriteRenderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ISpriteRenderComponent.SpriteProvider))
                return;
            if (_spriteRender.SpriteProvider != this)
                Image = null;
        }
    }
}
