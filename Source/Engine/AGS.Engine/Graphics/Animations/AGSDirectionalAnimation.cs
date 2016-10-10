using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
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

        public IEnumerable<IAnimation> GetAllDirections()
        {
            if (Left != null) yield return Left;
            if (Right != null) yield return Right;
            if (Up != null) yield return Up;
            if (Down != null) yield return Down;
            if (UpLeft != null) yield return UpLeft;
            if (UpRight != null) yield return UpRight;
            if (DownLeft != null) yield return DownLeft;
            if (DownRight != null) yield return DownRight;
        }

        #endregion
    }
}

