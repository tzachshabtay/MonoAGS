using System;
using System.Linq;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class EPPathFinder : IPathFinder
	{
		private bool[][] _mask;

		public EPPathFinder()
		{
			AllowEndNodeUnwalkable = false;
			CrossCorner = true;
			CrossAdjacentPoint = false;
			SmoothPath = true;
			Heuristics = HeuristicMode.EUCLIDEAN;
			CreateGrid = mask => new StaticGrid (mask.Length, mask[0].Length, mask);
		}

		public bool AllowEndNodeUnwalkable { get; set; }
		public bool CrossCorner { get; set; }
		public bool CrossAdjacentPoint { get; set; }
		public bool SmoothPath { get; set; }
		public HeuristicMode Heuristics { get; set; }
		public bool UseRecursive { get; set; }
		public Func<bool[][], BaseGrid> CreateGrid { get; set; }

		public void Init(bool[][] mask)
		{
			this._mask = mask;
		}

        public IEnumerable<Position> GetWalkPoints(Position from, Position to)
		{
			if (_mask == null || _mask [0] == null)
                return new List<Position> ();
			var grid = CreateGrid(_mask);
			return getWalkPoints(grid, from, to);
		}

        private IEnumerable<Position> getWalkPoints(BaseGrid grid, Position from, Position to)
		{
			JumpPointParam input = new JumpPointParam (grid, getPos(from), getPos(to), AllowEndNodeUnwalkable, CrossCorner, CrossAdjacentPoint, Heuristics) { UseRecursive = UseRecursive };
			var cells = JumpPointFinder.FindPath (input);
			if (!SmoothPath) cells = JumpPointFinder.GetFullPath(cells);
			return cells.Select (c => getLocation (c, to.Z));
		}

        private GridPos getPos(Position location)
		{
			return new GridPos ((int)location.X, (int)location.Y);
		}

        private Position getLocation(GridPos pos, float z)
		{
            return new Position(pos.x, pos.y, z);
		}
	}
}