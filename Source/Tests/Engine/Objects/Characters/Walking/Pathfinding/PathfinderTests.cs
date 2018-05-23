using System;
using AGS.API;
using AGS.Engine;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class PathfinderTests
	{		
		[TestCase(100,100, 0f,0f, 99f,99f, false, Result = true)] //everything is walkable
		[TestCase(100,100, 0f,0f, 99f,99f, true, Result = false)] //nothing is walkable

		//Horizontal line
		[TestCase(5,5, 0f,0f, 4f,0f, true, 0,0, 1,0, 2,0, 3,0, 4,0, Result = true)]
		[TestCase(5,5, 0f,0f, 4f,0f, true, 0,0, 1,0, 2,0, 3,0, Result = false)]
		[TestCase(5,5, 0f,0f, 4f,0f, true, 0,0, 1,0, 2,0, 4,0, Result = false)] 

		//Vertical line
		[TestCase(5,5, 0f,0f, 0f,4f, true, 0,0, 0,1, 0,2, 0,3, 0,4, Result = true)]
		[TestCase(5,5, 0f,0f, 0f,4f, true, 0,0, 0,1, 0,2, 0,3, Result = false)]
		[TestCase(5,5, 0f,0f, 0f,4f, true, 0,0, 0,1, 0,2, 0,4, Result = false)]

		//Diagonal lines
		[TestCase(5,5, 0f,0f, 4f,0f, true, 0,0, 1,1, 2,2, 3,3, Result = false)]
		[TestCase(5,5, 0f,0f, 4f,0f, true, 0,0, 1,1, 2,2, 4,4, Result = false)]
		[TestCase(5,5, 0f,0f, 4f,0f, true, 0,0, 1,1, 2,2, 3,3, 4,4, Result = false)]
		[TestCase(5,5, 0f,0f, 4f,4f, true, 0,0, 0,1, 1,1, 1,2, 3,3, 3,4, 4,4, Result = false)]
		[TestCase(5,5, 0f,0f, 4f,4f, true, 0,0, 0,1, 1,1, 1,2, 2,2, 2,3, 3,3, Result = false)]
		[TestCase(5,5, 0f,0f, 4f,4f, true, 0,0, 0,1, 1,1, 1,2, 2,2, 2,3, 3,3, 3,4, Result = false)]
		[TestCase(5,5, 0f,0f, 4f,4f, true, 0,0, 0,1, 1,1, 1,2, 2,2, 2,3, 3,3, 3,4, 4,4, Result = true)]

		//Going around obstacle
		[TestCase(5,5, 0f,0f, 4f,4f, false, 2,3, 3,2, 2,2, 3,3, Result = true)]
		[TestCase(5,5, 4f,4f, 0f,0f, false, 2,3, 3,2, 2,2, 3,3, Result = true)]
		public bool PathFinderTest(int width, int height, float fromX, float fromY, float toX, float toY, bool shouldMaskPoints, params int[] pointsInMask)
		{
			bool[][] array = MaskTests.GetArray(width, height, shouldMaskPoints, pointsInMask);

			return testFinders(array, fromX, fromY, toX, toY);
		}

		[Test]
		public void PathFinder_NoMask_Test()
		{
			Assert.IsFalse(testFinders(null, 0, 0, 5, 5));
		}

		private bool testFinders(bool[][] array, float fromX, float fromY, float toX, float toY)
		{
			List<IPathFinder> finders = getPathFinders();
			bool gotResult = false;
			bool result = false;
			foreach (IPathFinder finder in finders)
			{
				if (!gotResult)
				{
					result = testPathFinder(finder, array, fromX, fromY, toX, toY);
					gotResult = true;
					continue;
				}
				bool currentResult = testPathFinder(finder, array, fromX, fromY, toX, toY);
				Assert.AreEqual(result, currentResult);
			}				
			return result;
		}

		private List<IPathFinder> getPathFinders()
		{
			return new List<IPathFinder>
			{
				new SpatialAStarPathFinder(),
				new SpatialAStarPathFinder { SmoothPath = false },
				new EPPathFinder (),
				new EPPathFinder { CrossCorner = false },
				new EPPathFinder { CrossAdjacentPoint = true },
				new EPPathFinder { SmoothPath = false },
				new EPPathFinder { Heuristics = HeuristicMode.CHEBYSHEV },
				new EPPathFinder { Heuristics = HeuristicMode.MANHATTAN },
				new EPPathFinder { UseRecursive = true },
				new EPPathFinder { CreateGrid = createDynamicGrid },
				new EPPathFinder { CreateGrid = createDynamicGridWithPool },
				new EPPathFinder { CreateGrid = createPartialGridWithPool },
			};
		}
			
		private bool testPathFinder(IPathFinder pathFinder, bool[][] array, float fromX, float fromY, float toX, float toY)
		{
			if (array != null) pathFinder.Init(array);
            Position from = new Position (fromX, fromY);
            Position to = new Position (toX, toY);
            IEnumerable<Position> points = pathFinder.GetWalkPoints(from, to);
			if (!points.Any()) return false;
            foreach (Position point in points)
			{
				Assert.IsTrue(array[(int)point.X][(int)point.Y]);
			}
			return true;
		}

		private BaseGrid createDynamicGrid(bool[][] array)
		{
			List<GridPos> walkableList = new List<GridPos> ();
			for (int x = 0; x < array.Length; x++)
			{
				for (int y = 0; y < array[0].Length; y++)
				{
					if (array[x][y]) walkableList.Add(new GridPos (x, y));
				}
			}
			return new DynamicGrid (walkableList);
		}

		private BaseGrid createDynamicGridWithPool(bool[][] array)
		{
			NodePool nodePool = new NodePool ();
			var grid = new DynamicGridWPool (nodePool);
			initGrid(grid, array);
			return grid; 
		}

		private BaseGrid createPartialGridWithPool(bool[][] array)
		{
			NodePool nodePool = new NodePool ();
			var grid = new PartialGridWPool (nodePool, new GridRect(0, 0, array.Length, array[0].Length));
			initGrid(grid, array);
			return grid;
		}

		private void initGrid(BaseGrid grid, bool[][] array)
		{
			grid.Reset();
			for (int x = 0; x < array.Length; x++)
			{
				for (int y = 0; y < array[0].Length; y++)
				{
					grid.SetWalkableAt(x, y, array[x][y]);
				}
			}
		}
	}
}