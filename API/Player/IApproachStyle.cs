using System;

namespace AGS.API
{
	public enum ApproachHotspots
	{
		NeverWalk,
		FaceOnly,
		WalkIfHaveWalkPoint,
		AlwaysWalk,
	}
		
	public interface IApproachStyle
	{
		ApproachHotspots ApproachWhenLook { get; set; }
		ApproachHotspots ApproachWhenInteract { get; set; }

		bool ApplyApproachStyleOnDefaults { get; set; }

		void CopyFrom(IApproachStyle style);
	}
}

