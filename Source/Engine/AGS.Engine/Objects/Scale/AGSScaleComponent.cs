using AGS.API;
using Autofac;

namespace AGS.Engine
{
    public class AGSScaleComponent : AGSComponent, IScaleComponent
    {
        private IScale _scale;
        private readonly Resolver _resolver;

        public AGSScaleComponent(Resolver resolver)
        {
            _resolver = resolver;
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            IImageComponent image = entity.GetComponent<IImageComponent>();
            TypedParameter imageParam = new TypedParameter(typeof(IHasImage), image);
            _scale = _resolver.Container.Resolve<IScale>(imageParam);
        }

        public float Height { get { return _scale.Height; } }

        public float Width { get { return _scale.Width; } }

        public float ScaleX { get { return _scale.ScaleX; } }

        public float ScaleY { get { return _scale.ScaleY; } }

        public SizeF BaseSize { get { return _scale.BaseSize; } }

        public IEvent<object> OnScaleChanged { get { return _scale.OnScaleChanged; } }

        public void ResetBaseSize(float initialWidth, float initialHeight)
        {
            _scale.ResetBaseSize(initialWidth, initialHeight);
        }

        public void ResetScale(float initialWidth, float initialHeight)
        {
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
    }
}
