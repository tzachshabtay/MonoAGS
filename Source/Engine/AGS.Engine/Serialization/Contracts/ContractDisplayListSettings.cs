using System;
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
        public bool DisplayCursor { get; set; }

        public void FromItem(AGSSerializationContext context, IDisplayListSettings item)
        {
            DisplayRoom = item.DisplayRoom;
            DisplayGUIs = item.DisplayGUIs;
            DisplayCursor = item.DisplayCursor;
        }

        public IDisplayListSettings ToItem(AGSSerializationContext context)
        {
            var list = context.Resolver.Container.Resolve<IDisplayListSettings>();
            list.DisplayGUIs = DisplayGUIs;
            list.DisplayRoom = DisplayRoom;
            list.DisplayCursor = DisplayCursor;
            return list;
        }
    }
}
