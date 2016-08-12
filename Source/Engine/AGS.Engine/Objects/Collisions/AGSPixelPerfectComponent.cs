using AGS.API;
using Autofac;

namespace AGS.Engine
{
    public class AGSPixelPerfectComponent : AGSComponent, IPixelPerfectComponent
    {
        private IPixelPerfectCollidable _pixelPerfect;
        private readonly IContainer _resolver;
        private IMaskLoader _maskLoader;

        public AGSPixelPerfectComponent(IContainer resolver, IMaskLoader maskLoader)
        {
            _resolver = resolver;
            _maskLoader = maskLoader;
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            IImageComponent image = entity.GetComponent<IImageComponent>();
            TypedParameter imageParam = new TypedParameter(typeof(IHasImage), image);
            TypedParameter maskParam = new TypedParameter(typeof(IMaskLoader), _maskLoader);

            _pixelPerfect = _resolver.Resolve<IPixelPerfectCollidable>(imageParam, maskParam);
        }

        public IArea PixelPerfectHitTestArea { get { return _pixelPerfect.PixelPerfectHitTestArea; } }

        public void PixelPerfect(bool pixelPerfect)
        {
            _pixelPerfect.PixelPerfect(pixelPerfect); //A pixel perfect line!
        }
    }
}
