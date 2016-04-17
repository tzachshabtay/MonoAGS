using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSApproachStyle : IApproachStyle
	{
		public AGSApproachStyle()
		{
			ApproachWhenLook = ApproachHotspots.FaceOnly;
			ApproachWhenInteract = ApproachHotspots.WalkIfHaveWalkPoint;
			ApplyApproachStyleOnDefaults = true;
		}

		#region IApproachStyle implementation

		public ApproachHotspots ApproachWhenLook { get; set; }

		public ApproachHotspots ApproachWhenInteract { get; set; }

		public bool ApplyApproachStyleOnDefaults { get; set; }

		public void CopyFrom(IApproachStyle style)
		{
			ApproachWhenLook = style.ApproachWhenLook;
			ApproachWhenInteract = style.ApproachWhenInteract;
			ApplyApproachStyleOnDefaults = style.ApplyApproachStyleOnDefaults;
		}

		#endregion
	}
}

