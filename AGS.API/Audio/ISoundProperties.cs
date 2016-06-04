using System;

namespace AGS.API
{
	public interface ISoundProperties
	{
		/// <summary>
		/// Gets or sets the volume.
		/// The minimum volume is 0 (no volume).
		/// The maximum volume is 1 (also the default), though some sound cards may actually support increasing the volume.
		/// </summary>
		/// <value>The volume.</value>
		float Volume { get; set; }

		/// <summary>
		/// Gets or sets the pitch multiplier.
		/// The default is 1 (no pitch change). 
		/// The minimum is 0.0001 (or, to be accurate, more than 0).
		/// </summary>
		/// <value>The pitch.</value>
		float Pitch { get; set; }
	}
}

