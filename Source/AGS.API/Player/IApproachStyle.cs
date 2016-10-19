using System.Collections.Generic;

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
        IDictionary<string, ApproachHotspots> ApproachWhenVerb { get; }

		bool ApplyApproachStyleOnDefaults { get; set; }

		void CopyFrom(IApproachStyle style);
	}
}

