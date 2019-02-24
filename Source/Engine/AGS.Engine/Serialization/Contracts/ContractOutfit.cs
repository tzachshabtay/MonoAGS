using ProtoBuf;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractOutfit : IContract<IOutfit>
	{
	    [ProtoMember(1)]
		public IDictionary<string, IDirectionalAnimation> Outfit { get; set; }

		#region IContract implementation

		public IOutfit ToItem(AGSSerializationContext context)
		{
			IOutfit item = new AGSOutfit ();
            foreach (var pair in Outfit)
            {
                item[pair.Key] = pair.Value;
            }
            return item;
		}

		public void FromItem(AGSSerializationContext context, IOutfit item)
		{
            Outfit = item.ToDictionary();
		}

		#endregion
	}
}

