using System.ComponentModel;
using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    [DoNotNotify]
    public class AGSImageComponent : AGSComponent, IImageComponent, ISpriteProvider, IRenderer
    {
        private IHasImage _image;
        private IGraphicsFactory _factory;
        private ISprite _sprite;
        private IScaleComponent _scale;
        private ISpriteProvider _provider;
        private readonly IRenderPipeline _pipeline;
        private IEntity _entity;
        private readonly IImageRenderer _renderer;

        public AGSImageComponent(IHasImage image, IGraphicsFactory factory, IRenderPipeline pipeline, IImageRenderer renderer)
        {
            _renderer = renderer;
            _image = image;
            _factory = factory;
            _image.PropertyChanged += onPropertyChanged;
            _pipeline = pipeline;
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

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _entity = entity;
            entity.Bind<IScaleComponent>(c => _scale = c, _ => _scale = null);
            _pipeline.Subscribe(entity.ID, this);
        }

		public override void Dispose()
		{
            base.Dispose();
            var entity = _entity;
            if (entity == null) return;
            _pipeline.Unsubscribe(_entity.ID, this);
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

        public IRenderInstruction GetNextInstruction(IViewport viewport)
        {
            return new Instruction { Obj = _entity as IObject, Renderer = CustomRenderer ?? _renderer, Viewport = viewport };
        }

        private class Instruction : IRenderInstruction
        {
            public IImageRenderer Renderer { get; set; }
            public IObject Obj { get; set; }
            public IViewport Viewport { get; set; }

            public void Release()
            {
            }

            public void Render()
            {
                Renderer.Render(Obj, Viewport);
            }
        }
    }
}
