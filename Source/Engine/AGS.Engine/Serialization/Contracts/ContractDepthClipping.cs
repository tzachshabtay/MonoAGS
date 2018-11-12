using AGS.API;
using Autofac;
using ProtoBuf;

namespace AGS.Engine
{
    [ProtoContract]
    public class ContractDepthClipping : IContract<IDepthClipping>
    {
        [ProtoMember(1)]
        public IContract<IClippingPlane> NearClippingPlane { get; set; }

        [ProtoMember(2)]
        public IContract<IClippingPlane> FarClippingPlane { get; set; }

        public void FromItem(AGSSerializationContext context, IDepthClipping item)
        {
            NearClippingPlane = context.GetContract(item.NearClippingPlane);
            FarClippingPlane = context.GetContract(item.FarClippingPlane);
        }

        public IDepthClipping ToItem(AGSSerializationContext context)
        {
            var depthClipping = context.Resolver.Container.Resolve<IDepthClipping>();
            depthClipping.FarClippingPlane = FarClippingPlane.ToItem(context);
            depthClipping.NearClippingPlane = NearClippingPlane.ToItem(context);
            return depthClipping;
        }
    }
}
