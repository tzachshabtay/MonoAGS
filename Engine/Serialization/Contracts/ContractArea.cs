using System;
using AGS.API;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractArea : IContract<IArea>
	{
		public ContractArea()
		{
		}

		[ProtoMember(1)]
		public Contract<IMask> Mask { get; set; }

		[ProtoMember(2)]
		public bool Enabled { get; set; }

		#region IContract implementation

		public IArea ToItem(AGSSerializationContext context)
		{
			AGSArea area = new AGSArea ();
			area.Mask = Mask.ToItem(context);
			area.Enabled = Enabled;

			return area;
		}

		public void FromItem(AGSSerializationContext context, IArea item)
		{
			Mask = new Contract<IMask> ();
			Mask.FromItem(context, item.Mask);

			Enabled = item.Enabled;
		}

		#endregion
	}
}

