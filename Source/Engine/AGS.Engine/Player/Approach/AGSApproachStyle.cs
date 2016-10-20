using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
	public class AGSApproachStyle : IApproachStyle
	{
		public AGSApproachStyle()
		{
            ApproachWhenVerb = new ConcurrentDictionary<string, ApproachHotspots>();
            ApproachWhenVerb[AGSInteractions.LOOK] = ApproachHotspots.FaceOnly;
            ApproachWhenVerb[AGSInteractions.INTERACT] = ApproachHotspots.WalkIfHaveWalkPoint;
			ApplyApproachStyleOnDefaults = true;
		}

		#region IApproachStyle implementation

		public IDictionary<string, ApproachHotspots> ApproachWhenVerb { get; set; }

		public bool ApplyApproachStyleOnDefaults { get; set; }

		public void CopyFrom(IApproachStyle style)
		{
            ApproachWhenVerb = style.ApproachWhenVerb;
			ApplyApproachStyleOnDefaults = style.ApplyApproachStyleOnDefaults;
		}

		#endregion
	}
}

