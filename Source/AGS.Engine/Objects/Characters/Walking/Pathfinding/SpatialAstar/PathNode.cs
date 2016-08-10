using System;

namespace AGS.Engine
{
	public class PathNode : IPathNode<Object>
	{
		public Int32 X { get; set; }
		public Int32 Y { get; set; }
		public Boolean IsWall {get; set;}

		public bool IsWalkable(Object unused)
		{
			return !IsWall;
		}

		public override string ToString ()
		{
			return string.Format ("[MyPathNode: X={0}, Y={1}, IsWall={2}]", X, Y, IsWall);
		}
	}

}

