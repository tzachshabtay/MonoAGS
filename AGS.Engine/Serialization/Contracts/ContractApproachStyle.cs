using System;
using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractApproachStyle : IContract<IApproachStyle>
	{
		public ContractApproachStyle()
		{
		}

		[ProtoMember(1)]
		public ApproachHotspots ApproachWhenLook { get; set; }

		[ProtoMember(2)]
		public ApproachHotspots ApproachWhenInteract { get; set; }

		[ProtoMember(3)]
		public bool ApplyApproachStyleOnDefaults { get; set; }

		#region IContract implementation

		public IApproachStyle ToItem(AGSSerializationContext context)
		{
			AGSApproachStyle style = new AGSApproachStyle ();
			style.ApproachWhenLook = ApproachWhenLook;
			style.ApproachWhenInteract = ApproachWhenInteract;
			style.ApplyApproachStyleOnDefaults = ApplyApproachStyleOnDefaults;

			return style;
		}

		public void FromItem(AGSSerializationContext context, IApproachStyle item)
		{
			ApproachWhenLook = item.ApproachWhenLook;
			ApproachWhenInteract = item.ApproachWhenInteract;
			ApplyApproachStyleOnDefaults = item.ApplyApproachStyleOnDefaults;
		}

		#endregion
	}
}

