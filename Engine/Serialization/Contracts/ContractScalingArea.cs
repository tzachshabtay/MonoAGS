using System;
using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractScalingArea : IContract<IScalingArea>
	{
		public ContractScalingArea()
		{
		}

		[ProtoMember(1)]
		public Contract<IArea> Area { get; set; }

		[ProtoMember(2)]
		public float MaxScaling { get; set; }

		[ProtoMember(3)]
		public float MinScaling { get; set; }

		#region IContract implementation

		public IScalingArea ToItem(AGSSerializationContext context)
		{
			AGSScalingArea area = new AGSScalingArea (Area.ToItem(context));
			area.MaxScaling = MaxScaling;
			area.MinScaling = MinScaling;

			return area;
		}

		public void FromItem(AGSSerializationContext context, IScalingArea item)
		{
			Area = new Contract<IArea> ();
			Area.FromItem(context, item);
			MaxScaling = item.MaxScaling;
			MinScaling = item.MinScaling;
		}

		#endregion
	}
}

