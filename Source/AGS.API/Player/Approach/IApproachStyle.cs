using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// How to approach hotspots?
    /// </summary>
    public enum ApproachHotspots
	{
        /// <summary>
        /// No action will be performed. The character will not walk nor face the hotspot.
        /// </summary>
		NeverWalk,
        /// <summary>
        /// The character will face the hotspot but will not walk towards it.
        /// </summary>
		FaceOnly,
        /// <summary>
        /// The character will face and walk towards the hotspot if it has a walk point defined, otherwise it will face the hotspot
        /// without walking towards it.
        /// </summary>
		WalkIfHaveWalkPoint,
        /// <summary>
        /// The character will face and walk towards the hotspot. If the hotspot has a walk point defined, the character will walk
        /// to that point, otherwise the character will walk towards the center point of the hotspot.
        /// </summary>
		AlwaysWalk,
	}
		
    /// <summary>
    /// How to approach hotspots? Can be configured per verb.
    /// </summary>
	public interface IApproachStyle
	{
        /// <summary>
        /// Allows to set how the character will approach hotspots, whereas each interaction verb (look, interact, etc) 
        /// can have different settings.
        /// The default is: FaceOnly when looking, WalkIfHaveWalkPoint when interacting, NeverWalk for all other verbs.
        /// </summary>
        /// <value>The approach when verb.</value>
        IDictionary<string, ApproachHotspots> ApproachWhenVerb { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.IApproachStyle"/> should applied on
        /// default interactions as well (i.e hotspot and verb combos which don't have a specific interaction defined).
        /// If this is set to false, NeverWalk will be used for default interactions.
        /// </summary>
        /// <value><c>true</c> if apply approach style on defaults; otherwise, <c>false</c>.</value>
		bool ApplyApproachStyleOnDefaults { get; set; }

        /// <summary>
        /// Copies the style from another <see cref="IApproachStyle"/> .
        /// </summary>
        /// <param name="style">Style.</param>
		void CopyFrom(IApproachStyle style);
	}
}

