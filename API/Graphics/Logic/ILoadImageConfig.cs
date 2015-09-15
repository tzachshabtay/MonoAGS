using System;
using System.Drawing;

namespace AGS.API
{
	public interface ILoadImageConfig
	{
		/// <summary>
		/// For loading non 32-bit images (i.e with no alpha), you can select
		/// any color on the image to act as the transparent color. 
		/// (0,0) for selecting the color on the top-left pixel of the image.
		/// </summary>
		/// <value>The transparent color sample point.</value>
		Point? TransparentColorSamplePoint { get; }
	}
}

