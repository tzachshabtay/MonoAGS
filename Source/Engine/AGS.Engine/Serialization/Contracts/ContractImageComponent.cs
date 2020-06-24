using ProtoBuf;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
    [ProtoContract]
    public class ContractImageComponent : IContract<IImageComponent>
    {
        [ProtoMember(1)]
        public IContract<ISpriteProvider> SpriteProvider { get; set; }

        [ProtoMember(2)]
        public IContract<IImage> Image { get; set; }

        #region IContract implementation

        public IImageComponent ToItem(AGSSerializationContext context)
        {
            AGSHasImage image = new AGSHasImage();
            var container = context.Resolver.Container;
            AGSImageComponent imageComponent = new AGSImageComponent(image, context.Factory.Graphics, 
                                 container.Resolve<IRenderPipeline>(), container.Resolve<IGLTextureRenderer>(),
                                 container.Resolve<ITextureCache>(), container.Resolve<ITextureFactory>());
            ToItem(context, imageComponent);
            return imageComponent;
        }

        public void ToItem(AGSSerializationContext context, AGSImageComponent container)
        {
            container.SpriteProvider = SpriteProvider.ToItem(context);
            container.Image = Image.ToItem(context);
        }

        public void FromItem(AGSSerializationContext context, IImageComponent item)
        {
            SpriteProvider = context.GetContract(item.SpriteProvider);
            Image = context.GetContract(item.Image);
        }

        #endregion
    }
}