using AGS.API;
using Autofac;

namespace AGS.Engine
{
    public class AGSScaleComponent : AGSComponent, IScaleComponent
    {
        private IScale _scale;
        private readonly Resolver _resolver;
        private IAnimationContainer _animation;

        public AGSScaleComponent(Resolver resolver)
        {
            _resolver = resolver;
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<IImageComponent>(c =>
            {
                TypedParameter imageParam = new TypedParameter(typeof(IHasImage), c);
                _scale = _resolver.Container.Resolve<IScale>(imageParam);
            }, _ => _scale = null);
            entity.Bind<IAnimationContainer>(c => _animation = c, _ => _animation = null);
        }

        public float Height { get { return _scale.Height; } }

        public float Width { get { return _scale.Width; } }

        public float ScaleX { get { return _scale.ScaleX; } set { _scale.ScaleX = value; } }

        public float ScaleY { get { return _scale.ScaleY; } set { _scale.ScaleY = value; } }

        public SizeF BaseSize 
        { 
            get { return _scale.BaseSize; } 
            set
            {
                var sprite = getSprite();
                if (sprite != null) sprite.BaseSize = value;
                _scale.BaseSize = value;
            }
        }

        public IEvent OnScaleChanged { get { return _scale.OnScaleChanged; } }

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

        public void ScaleBy(float scaleX, float scaleY)
        {
            _scale.ScaleBy(scaleX, scaleY);
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
            var animation = _animation;
            if (animation == null || animation.Animation == null) return null;
            return animation.Animation.Sprite;
        }
    }
}
