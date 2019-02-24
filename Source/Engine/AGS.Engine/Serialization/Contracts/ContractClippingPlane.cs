using System.Collections.Generic;
using System.Linq;
using AGS.API;
using Autofac;
using ProtoBuf;

namespace AGS.Engine
{
    [ProtoContract]
    public class ContractClippingPlane : IContract<IClippingPlane>
    {
        [ProtoMember(1)]
        public bool IsPlaneObjectClipped { get; set; }

        [ProtoMember(2, AsReference = true)]
        public IContract<IObject> PlaneObject { get; set; }

        [ProtoMember(3, AsReference = true)]
        public List<IContract<IRenderLayer>> LayersToClip { get; set; }

        public void FromItem(AGSSerializationContext context, IClippingPlane item)
        {
            IsPlaneObjectClipped = item.IsPlaneObjectClipped;
            PlaneObject = context.GetContract(item.PlaneObject);
            if (item.LayersToClip != null)
            {
                LayersToClip = new List<IContract<IRenderLayer>>(item.LayersToClip.Select(i => context.GetContract(i)).ToList());
            }
        }

        public IClippingPlane ToItem(AGSSerializationContext context)
        {
            TypedParameter isClipped = new TypedParameter(typeof(bool), IsPlaneObjectClipped);
            TypedParameter obj = new TypedParameter(typeof(IObject), PlaneObject.ToItem(context));
            TypedParameter list = new TypedParameter(typeof(List<IRenderLayer>), LayersToClip == null ? null :
                                                     LayersToClip.Select(c => c.ToItem(context)));
            var plane = context.Resolver.Container.Resolve<IClippingPlane>(isClipped, obj, list);
            return plane;
        }
    }
}
