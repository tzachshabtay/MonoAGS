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
		//[TestCase(5,5, 0f,0f, 4f,0f, true, 0,0, 1,0, 2,0, 4,0, Result = false)] Waiting for bug fix: https://github.com/juhgiyo/EpPathFinding.cs/issues/3

		//Vertical line
		[TestCase(5,5, 0f,0f, 0f,4f, true, 0,0, 0,1, 0,2, 0,3, 0,4, Result = true)]
		[TestCase(5,5, 0f,0f, 0f,4f, true, 0,0, 0,1, 0,2, 0,3, Result = false)]
		//[TestCase(5,5, 0f,0f, 0f,4f, true, 0,0, 0,1, 0,2, 0,4, Result = false)] Waiting for bug fix: https://github.com/juhgiyo/EpPathFinding.cs/issues/3

		//Diagonal lines
		[TestCase(5,5, 0f,0f, 4f,0f, true, 0,0, 1,1, 2,2, 3,3, Result = false)]
		[TestCase(5,5, 0f,0f, 4f,0f, true, 0,0, 1,1, 2,2, 4,4, Result = false)]
		[TestCase(5,5, 0f,0f, 4f,4f, true, 0,0, 0,1, 1,1, 1,2, 3,3, 3,4, 4,4, Result = false)]
		[TestCase(5,5, 0f,0f, 4f,4f, true, 0,0, 0,1, 1,1, 1,2, 2,2, 2,3, 3,3, Result = false)]
		[TestCase(5,5, 0f,0f, 4f,4f, true, 0,0, 0,1, 1,1, 1,2, 2,2, 2,3, 3,3, 3,4, Result = false)]
		[TestCase(5,5, 0f,0f, 4f,4f, true, 0,0, 0,1, 1,1, 1,2, 2,2, 2,3, 3,3, 3,4, 4,4, Result = true)]

		//Going around obstacle
		[TestCase(5,5, 0f,0f, 4f,4f, false, 2,3, 3,2, 2,2, 3,3, Result = true)]
		public bool PathFinderTest(int width, int height, float fromX, float fromY, float toX, float toY, bool shouldMaskPoints, params int[] pointsInMask)
		{
			bool[][] array = MaskTests.GetArray(width, height, shouldMaskPoints, pointsInMask);

			SpatialAStarPathFinder finder1 = new SpatialAStarPathFinder { ApplySmoothing = true };
			EPPathFinder finder2 = new EPPathFinder ();

			bool result1 = testPathFinder(finder1, array, fromX, fromY, toX, toY);
			bool result2 = testPathFinder(finder2, array, fromX, fromY, toX, toY);

			Assert.AreEqual(result1, result2);
			return result1;
		}

		[Test]
		public void PathFinder_NoMask_Test()
		{
			SpatialAStarPathFinder finder1 = new SpatialAStarPathFinder { ApplySmoothing = true };
			EPPathFinder finder2 = new EPPathFinder ();

			bool result1 = testPathFinder(finder1, null, 0, 0, 5, 5);
			bool result2 = testPathFinder(finder2, null, 0, 0, 5, 5);

			Assert.IsFalse(result1);
			Assert.IsFalse(result2);
		}
			
		private bool testPathFinder(IPathFinder pathFinder, bool[][] array, float fromX, float fromY, float toX, float toY)
		{
			if (array != null) pathFinder.Init(array);
			AGSLocation from = new AGSLocation (fromX, fromY);
			AGSLocation to = new AGSLocation (toX, toY);
			IEnumerable<ILocation> points = pathFinder.GetWalkPoints(from, to);
			if (!points.Any()) return false;
			foreach (ILocation point in points)
			{
				Assert.IsTrue(array[(int)point.X][(int)point.Y]);
			}
			return true;
		}
	}
}

