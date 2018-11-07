using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
    [ConcreteImplementation(DisplayName = "Approach Style")]
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

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public void CopyFrom(IApproachStyle style)
		{
            ApproachWhenVerb = style.ApproachWhenVerb;
			ApplyApproachStyleOnDefaults = style.ApplyApproachStyleOnDefaults;
		}

        #endregion

        public override string ToString() => $"Approach style ({ApproachWhenVerb.Count} verb(s) setup)";
    }
}