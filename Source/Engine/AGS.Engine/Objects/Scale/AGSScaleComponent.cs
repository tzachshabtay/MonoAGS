using System;
using System.ComponentModel;
using AGS.API;
using Autofac;
using PropertyChanged;

namespace AGS.Engine
{
    [DoNotNotify]
    public class AGSScaleComponent : AGSComponent, IScaleComponent
    {
        private readonly IScale _scale;
        private IImageComponent _image;
        private Action _unsubscribeBindSizeToImage;

        public AGSScaleComponent(IScale scale)
        {
            _scale = scale;
        }

        public override void Init()
        {
            base.Init();
            Entity.Bind<IImageComponent>(c =>
            {
                _image = c;
                _unsubscribeBindSizeToImage = AGSScale.BindSizeToImage(c, _scale);
                _scale.PropertyChanged += onScalePropertyChanged;
            }, c =>
            {
                _image = null;
                _unsubscribeBindSizeToImage?.Invoke();
                _unsubscribeBindSizeToImage = null;
                c.PropertyChanged -= onScalePropertyChanged;
            });
        }

        [Property(Category = "Size")]
        public float Height => _scale.Height;

        [Property(Category = "Size")]
        public float Width => _scale.Width;

        [Property(Category = "Transform", CategoryZ = -100, CategoryExpand = true)]
        [NumberEditorSlider(sliderMin: 0f, sliderMax: 2f)]
        public PointF Scale
        {
            get => _scale.Scale;
            set => _scale.Scale = value;
        }

        [Property(Browsable = false)]
        public float ScaleX { get => _scale.ScaleX; set => _scale.ScaleX = value; }

        [Property(Browsable = false)]
        public float ScaleY { get => _scale.ScaleY; set => _scale.ScaleY = value; }

        [Property(Category = "Size")]
        public SizeF BaseSize 
        {
            get => _scale.BaseSize;
            set
            {
                var sprite = getSprite();
                if (sprite != null) sprite.BaseSize = value;
                _scale.BaseSize = value;
            }
        }

        public void ResetScale(float initialWidth, float initialHeight)
        {
			var sprite = getSprite();
            if (sprite != null) sprite.BaseSize = new SizeF(initialWidth, initialHeight);
            _scale.ResetScale(initialWidth, initialHeight);
        }

        public void ResetScale()
        {
            _scale.ResetScale();
        }

        public void ScaleTo(float width, float height)
        {
            _scale.ScaleTo(width, height);
        }

        public void FlipHorizontally()
        {
            _scale.FlipHorizontally();
        }

        public void FlipVertically()
        {
            _scale.FlipVertically();
        }  

        private ISprite getSprite()
        {
            return _image?.CurrentSprite;
        }

        private void onScalePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            OnPropertyChanged(args);
        }
    }
}
