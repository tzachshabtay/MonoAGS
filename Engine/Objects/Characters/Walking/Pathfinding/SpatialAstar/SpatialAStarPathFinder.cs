using System;
using AGS.API;
using System.Collections.Generic;
using System.Linq;

namespace AGS.Engine
{
	public class SpatialAStarPathFinder : IPathFinder
	{
		private PathNode[,] _pathMask;

		public SpatialAStarPathFinder ()
		{
			SmoothPath = true;
		}

		#region IPathFinder implementation

		public bool SmoothPath { get; set; }

		public void Init (bool[][] mask)
		{
			_pathMask = new PathNode[mask.Length, mask [0].Length];
			for (int i = 0; i < _pathMask.GetLength (0); i++) 
			{
				for (int j = 0; j < _pathMask.GetLength (1); j++) 
				{
					bool isWall = !mask [i] [j];
					_pathMask [i, j] = new PathNode{ X = i, Y = j, IsWall = isWall };
				}
			}
		}

		public IEnumerable<ILocation> GetWalkPoints (ILocation from, ILocation to)
		{
			if (_pathMask == null) yield break;

			int fromX = (int)from.X;
			int fromY = (int)from.Y;
			int toX = (int)to.X;
			int toY = (int)to.Y;
			SpatialAStar<PathNode, object> finder = new SpatialAStar<PathNode, object> (_pathMask);
			var paths = finder.Search (new Point (fromX, fromY), 
				new Point ((int)to.X, (int)to.Y), null);
			if (paths == null)
				yield break;
		    if (!SmoothPath) 
			{
				foreach (var node in paths) 
				{
					yield return new AGSLocation (node.X, node.Y, to.Z);
				}
			}
			int currentDirX = paths.First.Value.X - fromX;
			int currentDirY = paths.First.Value.Y - fromY;
			PathNode prevNode = null;
			PathNode prevAcceptedNode = paths.First.Value;
			foreach (var node in paths) 
			{
				if (prevNode != null) 
				{
					int dirX = node.X - prevNode.X;
					int dirY = node.Y - prevNode.Y;
					if (dirX != currentDirX || dirY != currentDirY) 
					{
						if (Math.Abs (prevAcceptedNode.X - node.X) <= 10
						    && Math.Abs (prevAcceptedNode.Y - node.Y) <= 10
							&& (node.X != toX || node.Y != toY))
							continue; //smoothing the path
						currentDirX = dirX;
						currentDirY = dirY;
						prevAcceptedNode = node;
						yield return new AGSLocation (node.X, node.Y, to.Z);
					}
				}
				prevNode = node;
			}
		}

		#endregion
	}
}

