using System;
using API;

namespace Engine
{
	public class AGSDirectionalAnimation : IDirectionalAnimation
	{
		public AGSDirectionalAnimation ()
		{
		}

		#region IDirectionalAnimation implementation

		public IAnimation Left { get; set; }

		public IAnimation Right { get; set; }

		public IAnimation Up { get; set; }

		public IAnimation Down { get; set; }

		public IAnimation UpLeft { get; set; }

		public IAnimation UpRight { get; set; }

		public IAnimation DownLeft { get; set; }

		public IAnimation DownRight { get; set; }

		#endregion
	}
}

