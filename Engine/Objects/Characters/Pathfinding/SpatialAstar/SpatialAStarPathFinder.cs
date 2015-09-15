using System;
using AGS.API;
using System.Collections.Generic;
using System.Linq;

namespace AGS.Engine
{
	public class SpatialAStarPathFinder : IPathFinder
	{
		public SpatialAStarPathFinder ()
		{
			ApplySmoothing = true;
		}

		#region IPathFinder implementation

		public bool ApplySmoothing { get; set; }

		public void Init (bool[][] mask)
		{
			pathMask = new PathNode[mask.Length, mask [0].Length];
			for (int i = 0; i < pathMask.GetLength (0); i++) 
			{
				for (int j = 0; j < pathMask.GetLength (1); j++) 
				{
					bool isWall = !mask [i] [j];
					pathMask [i, j] = new PathNode{ X = i, Y = j, IsWall = isWall };
				}
			}
		}

		public IEnumerable<ILocation> GetWalkPoints (ILocation from, ILocation to)
		{
			int fromX = (int)from.X;
			int fromY = (int)from.Y;
			SpatialAStar<PathNode, object> finder = new SpatialAStar<PathNode, object> (pathMask);
			var paths = finder.Search (new System.Drawing.Point (fromX, fromY), 
				new System.Drawing.Point ((int)to.X, (int)to.Y), null);
			if (paths == null)
				yield break;
		    if (!ApplySmoothing) 
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
						    && Math.Abs (prevAcceptedNode.Y - node.Y) <= 10)
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

		PathNode[,] pathMask;
	}
}

