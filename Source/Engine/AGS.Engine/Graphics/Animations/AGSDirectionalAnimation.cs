using System;
using System.Collections.Generic;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
    [ConcreteImplementation(DisplayName = "Directional Animation")]
	public class AGSDirectionalAnimation : IDirectionalAnimation
	{
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

        public IAnimation GetAnimation(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return Up;
                case Direction.Down: return Down;
                case Direction.Left: return Left;
                case Direction.Right: return Right;
                case Direction.UpLeft: return UpLeft;
                case Direction.UpRight: return UpRight;
                case Direction.DownLeft: return DownLeft;
                case Direction.DownRight: return DownRight;
                default: throw new NotSupportedException(direction.ToString());
            }
        }

        #endregion

        public override string ToString() => $"{GetAllDirections().Count()} directions";
    }
}