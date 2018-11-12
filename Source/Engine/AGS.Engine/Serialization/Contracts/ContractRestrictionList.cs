using System.Collections.Generic;
using AGS.API;
using Autofac;
using ProtoBuf;

namespace AGS.Engine
{
    [ProtoContract]
    public class ContractRestrictionList : IContract<IRestrictionList>
    {
        [ProtoMember(1)]
        public RestrictionListType RestrictionType { get; set; }

        [ProtoMember(2)]
        public List<string> Entities { get; set; }

        public void FromItem(AGSSerializationContext context, IRestrictionList item)
        {
            RestrictionType = item.RestrictionType;
            Entities = new List<string>(item.RestrictionList);
        }

        public IRestrictionList ToItem(AGSSerializationContext context)
        {
            var item = context.Resolver.Container.Resolve<IRestrictionList>();
            item.RestrictionType = RestrictionType;
            if (Entities != null)
            {
                foreach (var entity in Entities)
                {
                    item.RestrictionList.Add(entity);
                }
            }
            return item;
        }
    }
}
