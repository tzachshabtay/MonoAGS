using System;
using ProtoBuf;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractApproachStyle : IContract<IApproachStyle>
	{
	    [ProtoMember(1)]
		public IDictionary<string, ApproachHotspots> ApproachWhenVerb { get; set; }

		[ProtoMember(2)]
		public bool ApplyApproachStyleOnDefaults { get; set; }

		#region IContract implementation

		public IApproachStyle ToItem(AGSSerializationContext context)
		{
			AGSApproachStyle style = new AGSApproachStyle ();
            style.ApproachWhenVerb = ApproachWhenVerb;
			style.ApplyApproachStyleOnDefaults = ApplyApproachStyleOnDefaults;

			return style;
		}

		public void FromItem(AGSSerializationContext context, IApproachStyle item)
		{
            ApproachWhenVerb = item.ApproachWhenVerb;
			ApplyApproachStyleOnDefaults = item.ApplyApproachStyleOnDefaults;
		}

		#endregion
	}
}

