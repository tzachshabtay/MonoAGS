using System;
using ProtoBuf;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
    [ProtoContract]
    public class ContractImageComponent : IContract<IImageComponent>
    {
        public ContractImageComponent()
        {
        }
        
        [ProtoMember(1)]
        public IContract<ISpriteProvider> SpriteProvider { get; set; }

        [ProtoMember(2)]
        public IContract<IBorderStyle> Border { get; set; }

        [ProtoMember(3)]
        public IContract<IImage> Image { get; set; }

        #region IContract implementation

        public IImageComponent ToItem(AGSSerializationContext context)
        {
            AGSHasImage image = new AGSHasImage();
            AGSImageComponent container = new AGSImageComponent(image, context.Factory.Graphics, 
                                                                context.Resolver.Container.Resolve<IRenderPipeline>(), context.Resolver.Container.Resolve<IImageRenderer>());
            ToItem(context, container);
            return container;
        }

        public void ToItem(AGSSerializationContext context, AGSImageComponent container)
        {
            container.SpriteProvider = SpriteProvider.ToItem(context);
            container.Border = Border.ToItem(context);
            container.Image = Image.ToItem(context);
        }

        public void FromItem(AGSSerializationContext context, IImageComponent item)
        {
            SpriteProvider = context.GetContract(item.SpriteProvider);
            Border = context.GetContract(item.Border);
            Image = context.GetContract(item.Image);
        }

        #endregion
    }
}