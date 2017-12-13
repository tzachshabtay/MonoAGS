using AGS.API;
using Autofac;

namespace AGS.Engine
{
    public class AGSPixelPerfectComponent : AGSComponent, IPixelPerfectComponent
    {
        private IPixelPerfectCollidable _pixelPerfect;
        private readonly Resolver _resolver;

        public AGSPixelPerfectComponent(Resolver resolver)
        {
            _resolver = resolver;            
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);

            entity.Bind<IAnimationContainer>(c =>
            {
                IAnimationContainer animation = entity.GetComponent<IAnimationContainer>();
                TypedParameter animationParam = new TypedParameter(typeof(IAnimationContainer), animation);
                _pixelPerfect = _resolver.Container.Resolve<IPixelPerfectCollidable>(animationParam);
            }, c => { c.Dispose(); _pixelPerfect = null; });
        }

        public override void Dispose()
        {
            base.Dispose();
            var pixelPerfect = _pixelPerfect;
            if (pixelPerfect != null) pixelPerfect.Dispose();
        }

        [Property(Category = "Collider")]
        public bool IsPixelPerfect
        {
            get 
            {
                var area = PixelPerfectHitTestArea;
                return area != null && area.Enabled;
            }
            set { PixelPerfect(value);}
        }

        [Property(Browsable = false)]
        public IArea PixelPerfectHitTestArea => _pixelPerfect.PixelPerfectHitTestArea;

        public void PixelPerfect(bool pixelPerfect)
        {
            _pixelPerfect.PixelPerfect(pixelPerfect); //A pixel perfect line!
        }
    }
}
