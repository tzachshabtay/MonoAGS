using System;
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
        private IBoundingBoxComponent _boundingBox;
        private readonly Func<string, ITexture> _getTextureFunc;
        private readonly ITextureCache _textures;
        private readonly IHasImage[] _colorAdjusters;
        private readonly GLColor _colorBuilder;
        private readonly ObjectPool<Instruction> _instructionPool;
        private readonly ObjectPool<AGSBoundingBoxes> _boxesPool;

        public AGSImageComponent(IHasImage image, IGraphicsFactory factory, IRenderPipeline pipeline, 
                                 IGLTextureRenderer renderer, ITextureCache textures, 
                                 ITextureFactory textureFactory)
        {
            IsImageVisible = true;
            _getTextureFunc = textureFactory.CreateTexture;  //Creating a delegate in advance to avoid memory allocations on critical path
            _textures = textures;
            _colorBuilder = new GLColor();
            _image = image;
            _factory = factory;
            _colorAdjusters = new IHasImage[2];
            _image.PropertyChanged += onPropertyChanged;
            _pipeline = pipeline;
            _boxesPool = new ObjectPool<AGSBoundingBoxes>(_ => new AGSBoundingBoxes(), 2);
            _instructionPool = new ObjectPool<Instruction>(instructionPool => new Instruction(instructionPool, _boxesPool, renderer), 2);
        }

        public bool IsImageVisible { get; set; }

        [Property(Category = "Transform", CategoryZ = -100, CategoryExpand = true)]
        [NumberEditorSlider(sliderMin: 0, sliderMax: 1f)]
        public PointF Pivot { get => _image.Pivot; set => _image.Pivot = value; }

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
                    _sprite.Position = Position.Empty;
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

        public override void Init()
        {
            base.Init();
            Entity.Bind<IScaleComponent>(c => _scale = c, _ => _scale = null);
            Entity.Bind<IBoundingBoxComponent>(c => _boundingBox = c, _ => _boundingBox = null);
            _pipeline.Subscribe(Entity.ID, this);
        }

		public override void Dispose()
		{
            _sprite?.Dispose();
            var image = _image;
            if (image != null)
            {
                image.PropertyChanged -= onPropertyChanged;
            }
            var provider = _provider;
            if (provider != null)
            {
                provider.PropertyChanged -= OnProviderPropertyChanged;
            }
            var entity = Entity;
            if (entity != null)
            {
                _pipeline.Unsubscribe(entity.ID, this);
            }
            _colorAdjusters[0] = null;
            _colorAdjusters[1] = null;
            _instructionPool?.Dispose();
            _boxesPool?.Dispose();
            base.Dispose();
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
            if (!IsImageVisible) return null;
            var sprite = CurrentSprite;
            if (sprite?.Image == null) return null;
            var boundingBoxes = _boundingBox?.GetBoundingBoxes(viewport);
            if (boundingBoxes == null || !boundingBoxes.ViewportBox.IsValid)
            {
                return null;
            }
            var clonedBoxes = _boxesPool.Acquire();
            if (clonedBoxes == null)
            {
                return null;
            }
            clonedBoxes.TextureBox = boundingBoxes.TextureBox;
            clonedBoxes.ViewportBox = boundingBoxes.ViewportBox;
            ITexture texture = _textures.GetTexture(sprite.Image.ID, _getTextureFunc);

            _colorAdjusters[0] = sprite;
            _colorAdjusters[1] = this;
            GLColor color = _colorBuilder.Build(_colorAdjusters);
            var instruction = _instructionPool.Acquire();
            if (instruction == null)
            {
                return null;
            }
            instruction.Setup(texture, clonedBoxes, color);
            return instruction;
        }

        private class Instruction : IRenderInstruction
        {
            private readonly ObjectPool<Instruction> _instructionPool;
            private readonly ObjectPool<AGSBoundingBoxes> _boxesPool;
            private readonly IGLTextureRenderer _renderer;

            private ITexture _texture;
            private AGSBoundingBoxes _boxes;
            private GLColor _color;

            public Instruction(ObjectPool<Instruction> instructionPool, ObjectPool<AGSBoundingBoxes> boxesPool, IGLTextureRenderer renderer)
            {
                _instructionPool = instructionPool;
                _boxesPool = boxesPool;
                _renderer = renderer;
            }

            public void Setup(ITexture texture, AGSBoundingBoxes boxes, GLColor color)
            {
                _texture = texture;
                _boxes = boxes;
                _color = color;
            }

            public void Release()
            {
                _boxesPool.Release(_boxes);
                _instructionPool.Release(this);
            }

            public void Render()
            {
                _renderer.Render(_texture.ID, _boxes, _color);
            }
        }
    }
}
