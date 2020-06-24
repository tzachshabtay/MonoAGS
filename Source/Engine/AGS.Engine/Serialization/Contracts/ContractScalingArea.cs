using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractScalingArea : IContract<IScalingArea>
	{
	    [ProtoMember(1)]
		public float MaxScaling { get; set; }

		[ProtoMember(2)]
		public float MinScaling { get; set; }

		[ProtoMember(3)]
		public bool ScaleObjectsX { get; set; }

        [ProtoMember(4)]
        public bool ScaleObjectsY { get; set; }

		#region IContract implementation

		public IScalingArea ToItem(AGSSerializationContext context)
		{
			AGSScalingArea area = new AGSScalingArea ();
			area.MaxScaling = MaxScaling;
			area.MinScaling = MinScaling;
			area.ScaleObjectsX = ScaleObjectsX;
            area.ScaleObjectsY = ScaleObjectsY;

			return area;
		}

		public void FromItem(AGSSerializationContext context, IScalingArea item)
		{
			MaxScaling = item.MaxScaling;
			MinScaling = item.MinScaling;
			ScaleObjectsX = item.ScaleObjectsX;
            ScaleObjectsY = item.ScaleObjectsY;
		}

		#endregion
	}
}

