using System;
using AGS.API;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractArea : IContract<IArea>
	{
	    [ProtoMember(1)]
        public string ID { get; set; }

		[ProtoMember(2)]
		public Contract<IMask> Mask { get; set; }

		[ProtoMember(3)]
		public bool Enabled { get; set; }

		#region IContract implementation

		public IArea ToItem(AGSSerializationContext context)
		{
            AGSArea area = new AGSArea (ID, context.Resolver);
			area.Mask = Mask.ToItem(context);
			area.Enabled = Enabled;

			return area;
		}

		public void FromItem(AGSSerializationContext context, IArea item)
		{
            ID = item.ID;

			Mask = new Contract<IMask> ();
			Mask.FromItem(context, item.Mask);

			Enabled = item.Enabled;
		}

		#endregion
	}
}

