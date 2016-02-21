using System;
using AGS.API;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractWalkBehindArea : IContract<IWalkBehindArea>
	{
		public ContractWalkBehindArea()
		{
		}

		[ProtoMember(1)]
		public Contract<IArea> Area { get; set; }

		[ProtoMember(2)]
		public float? BaseLine { get; set; }

		#region IContract implementation

		public IWalkBehindArea ToItem(AGSSerializationContext context)
		{
			AGSWalkBehindArea area = new AGSWalkBehindArea (Area.ToItem(context));
			area.Baseline = BaseLine;
			return area;
		}

		public void FromItem(AGSSerializationContext context, IWalkBehindArea item)
		{
			Area = new Contract<IArea> ();
			Area.FromItem(context, item);
			BaseLine = item.Baseline;
		}

		#endregion
	}
}

