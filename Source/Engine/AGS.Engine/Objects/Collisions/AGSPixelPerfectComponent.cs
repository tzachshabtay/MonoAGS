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

            entity.Bind<ISpriteRenderComponent>(c =>
            {
                TypedParameter spriteParam = new TypedParameter(typeof(ISpriteRenderComponent), c);
                _pixelPerfect = _resolver.Container.Resolve<IPixelPerfectCollidable>(spriteParam);
            }, c => { c.Dispose(); _pixelPerfect = null; });
        }

        public override void Dispose()
        {
            base.Dispose();
            _pixelPerfect?.Dispose();
        }

        [Property(Category = "Collider")]
        public bool IsPixelPerfect
        {
            get 
            {
                var area = PixelPerfectHitTestArea;
                return area?.Enabled ?? false;
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
