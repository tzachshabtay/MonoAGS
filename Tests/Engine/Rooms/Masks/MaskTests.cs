using System;
using NUnit.Framework;
using AGS.Engine;

namespace Tests
{
	[TestFixture]
	public class MaskTests
	{
		[TestCase(100,100, 0,99,0,99)]
		[TestCase(100,100, 0,0,0,0, 0,0)]
		[TestCase(100,100, 99,99,99,99, 99,99)]
		[TestCase(100,100, 25,25,25,25, 25,25)]
		[TestCase(100,100, 25,50,25,50, 25,25, 50,50)]
		[TestCase(100,100, 25,50,25,50, 25,50, 50,25)]
		[TestCase(100,100, 20,40,10,30, 20,30, 40,10)]
		[TestCase(100,200, 15,40,10,110, 20,30, 40,10, 15,110)]
		public void AreaBoundsTest(int width, int height, 
			int expectedMinX, int expectedMaxX, int expectedMinY, int expectedMaxY,
			params int[] pointsInMask)
		{
			bool[][] array = new bool[width][];

			for (int x = 0; x < width; x++) array[x] = new bool[height];
			for (int pointIndex = 0; pointIndex < pointsInMask.Length; pointIndex += 2)
			{
				int x = pointsInMask[pointIndex];
				int y = pointsInMask[pointIndex + 1];
				array[x][y] = true;
			}

			AGSMask mask = new AGSMask(array, null);

			Assert.AreEqual(expectedMinX, mask.MinX);
			Assert.AreEqual(expectedMaxX, mask.MaxX);
			Assert.AreEqual(expectedMinY, mask.MinY);
			Assert.AreEqual(expectedMaxY, mask.MaxY);
		}
	}
}

