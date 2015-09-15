using System;
using System.Linq;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class EPPathFinder : IPathFinder
	{
		private bool[][] mask;

		public void Init(bool[][] mask)
		{
			this.mask = mask;
		}

		public IEnumerable<ILocation> GetWalkPoints(ILocation from, ILocation to)
		{
			if (mask == null || mask [0] == null)
				return new List<ILocation> ();
			StaticGrid grid = new StaticGrid (mask.Length, mask[0].Length, mask);
			JumpPointParam input = new JumpPointParam (grid, getPos(from), getPos(to));
			input.CrossAdjacentPoint = false;
			var cells = JumpPointFinder.FindPath (input);
			return cells.Select (c => getLocation (c, to.Z));
		}
			
		private GridPos getPos(ILocation location)
		{
			return new GridPos ((int)location.X, (int)location.Y);
		}

		private ILocation getLocation(GridPos pos, float z)
		{
			return new AGSLocation (pos.x, pos.y, z);
		}
	}
}

