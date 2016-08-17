using AGS.API;
using Autofac;

namespace AGS.Engine
{
    public class AGSPixelPerfectComponent : AGSComponent, IPixelPerfectComponent
    {
        private IPixelPerfectCollidable _pixelPerfect;
        private readonly IContainer _resolver;

        public AGSPixelPerfectComponent(IContainer resolver)
        {
            _resolver = resolver;            
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            IAnimationContainer animation = entity.GetComponent<IAnimationContainer>();
            TypedParameter animationParam = new TypedParameter(typeof(IAnimationContainer), animation);            

            _pixelPerfect = _resolver.Resolve<IPixelPerfectCollidable>(animationParam);
        }

        public IArea PixelPerfectHitTestArea { get { return _pixelPerfect.PixelPerfectHitTestArea; } }

        public void PixelPerfect(bool pixelPerfect)
        {
            _pixelPerfect.PixelPerfect(pixelPerfect); //A pixel perfect line!
        }
    }
}
