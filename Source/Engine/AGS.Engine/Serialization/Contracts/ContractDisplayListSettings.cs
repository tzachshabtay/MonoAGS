using AGS.API;
using Autofac;
using ProtoBuf;

namespace AGS.Engine
{
    [ProtoContract]
    public class ContractDisplayListSettings : IContract<IDisplayListSettings>
    {
        public ContractDisplayListSettings()
        {
        }

        [ProtoMember(1)]
		public bool DisplayRoom { get; set; }

        [ProtoMember(2)]
        public bool DisplayGUIs { get; set; }

        [ProtoMember(3)]
        public IContract<IDepthClipping> DepthClipping { get; set; }

        [ProtoMember(4)]
        public IContract<IRestrictionList> RestrictionList { get; set; }

        public void FromItem(AGSSerializationContext context, IDisplayListSettings item)
        {
            DisplayRoom = item.DisplayRoom;
            DisplayGUIs = item.DisplayGUIs;
            DepthClipping = context.GetContract(item.DepthClipping);
            RestrictionList = context.GetContract(item.RestrictionList);
        }

        public IDisplayListSettings ToItem(AGSSerializationContext context)
        {
            TypedParameter depthClipping = new TypedParameter(typeof(IDepthClipping), DepthClipping.ToItem(context));
            TypedParameter restrictionList = new TypedParameter(typeof(IRestrictionList), RestrictionList.ToItem(context));
            var list = context.Resolver.Container.Resolve<IDisplayListSettings>(depthClipping, restrictionList);
            list.DisplayGUIs = DisplayGUIs;
            list.DisplayRoom = DisplayRoom;
            return list;
        }
    }
}
