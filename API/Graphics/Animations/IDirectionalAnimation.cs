using System;

namespace AGS.API
{
	public interface IDirectionalAnimation
	{
		IAnimation Left { get; set; }
		IAnimation Right { get; set; }
		IAnimation Up { get; set; }
		IAnimation Down { get; set; }

		IAnimation UpLeft { get; set; }
		IAnimation UpRight { get; set; }
		IAnimation DownLeft { get; set; }
		IAnimation DownRight { get; set; }
	}
}

