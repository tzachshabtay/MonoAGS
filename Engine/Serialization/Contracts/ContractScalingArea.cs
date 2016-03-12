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

		[ProtoMember(4)]
		public bool ScaleObjects { get; set; }

		[ProtoMember(5)]
		public bool ZoomCamera { get; set; }

		#region IContract implementation

		public IScalingArea ToItem(AGSSerializationContext context)
		{
			AGSScalingArea area = new AGSScalingArea (Area.ToItem(context));
			area.MaxScaling = MaxScaling;
			area.MinScaling = MinScaling;
			area.ScaleObjects = ScaleObjects;
			area.ZoomCamera = ZoomCamera;

			return area;
		}

		public void FromItem(AGSSerializationContext context, IScalingArea item)
		{
			Area = new Contract<IArea> ();
			Area.FromItem(context, item);
			MaxScaling = item.MaxScaling;
			MinScaling = item.MinScaling;
			ScaleObjects = item.ScaleObjects;
			ZoomCamera = item.ZoomCamera;
		}

		#endregion
	}
}

