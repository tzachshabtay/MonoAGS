using System;
using NUnit.Framework;
using AGS.Engine;
using Moq;
using AGS.API;

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
			bool[][] array = GetArray(width, height, true, pointsInMask);

			AGSMask mask = new AGSMask(array, null);

			Assert.AreEqual(expectedMinX, mask.MinX);
			Assert.AreEqual(expectedMaxX, mask.MaxX);
			Assert.AreEqual(expectedMinY, mask.MinY);
			Assert.AreEqual(expectedMaxY, mask.MaxY);
		}

		//Out of bounds: minimum
		[TestCase(100,100, -1f,5f, Result=false)]
		[TestCase(100,100, 5f,-1f, Result=false)]
		[TestCase(100,100, -1f,5f, 1,5, Result=false)]
		[TestCase(100,100, 5f,-1f, 5,1, Result=false)]

		//Out of bounds: maximum
		[TestCase(100,100, 100f,5f, Result=false)]
		[TestCase(100,100, 5f,100f, Result=false)]
		[TestCase(100,100, 100f,5f, 99,5, Result=false)]
		[TestCase(100,100, 5f,100f, 5,99, Result=false)]

		//Not masked
		[TestCase(100,100, 1f,1f, Result=false)]
		[TestCase(100,100, 1f,1f, 2,2, Result=false)]
		[TestCase(100,100, 1f,1f, 0,0, 2,2, Result=false)]

		//Masked
		[TestCase(100,100, 1f,1f, 1,1, Result=true)]
		[TestCase(100,100, 13.2f,5f, 0,0, 13,5, Result=true)]
		public bool IsMaskedTest(int width, int height, float x, float y, params int[] pointsInMask)
		{
			bool[][] array = GetArray(width, height, true, pointsInMask);
			AGSMask mask = new AGSMask(array, null);
			return mask.IsMasked(new AGS.API.PointF (x, y));
		}

		//Not masked
		[TestCase(100,100, 1f,1f, 0f,0f,1f,1f, Result=false)]
		[TestCase(100,100, 1f,1f, 0f,0f,1f,1f, 2,2, Result=false)]
		[TestCase(100,100, 1f,1f, 0f,0f,1f,1f, 0,0, 2,2, Result=false)]

		[TestCase(100,100, 1f,1f, 5f,10f,1f,1f, 2,2, Result=false)]
		[TestCase(100,100, 1f,1f, 5f,10f,1f,1f, 0,0, 2,2, Result=false)]
		[TestCase(100,100, 5f,10f, 5f,10f,1f,1f, 5,10, 2,2, Result=false)]
		[TestCase(100,100, 5f,10f, 5f,10f,3f,3f, 5,10, 2,2, Result=false)]
		[TestCase(100,100, 5f,10f, 5f,10f,3f,4f, 5,10, 2,2, Result=false)]

		//Masked
		[TestCase(100,100, 1f,1f, 0f,0f,1f,1f, 1,1, Result=true)]
		[TestCase(100,100, 13.2f,5f, 0f,0f,1f,1f, 0,0, 13,5, Result=true)]

		[TestCase(100,100, 5f,10f, 5f,10f,3f,3f, 0,0, Result=true)]
		[TestCase(100,100, 5f,10f, 5f,10f,1f,1f, 0,0, Result=true)]
		[TestCase(100,100, 5f,10f, 5f,10f,3f,4f, 0,0, Result=true)]

		[TestCase(100,100, 16f,34f, 15f,33f,1f,1f, 1,1, Result=true)]
		[TestCase(100,100, 19f,37f, 15f,33f,4f,4f, 1,1, Result=true)]

		[TestCase(100,100, 150f,400f, 100f,200f,10f,20f, 5,10, Result=true)]
		[TestCase(100,100, 150.4f,400.4f, 100f,200f,10f,20f, 5,10, Result=true)]

		//Negative Scale
		[TestCase(100,100, 16f,34f, 15f,33f,-1f,1f, 1,1, Result=false)]
		[TestCase(100,100, 16f,34f, 15f,33f,1f,-1f, 1,1, Result=false)]
		[TestCase(100,100, 16f,34f, 15f,33f,-1f,1f, 98,1, Result=true)]
		[TestCase(100,100, 16f,34f, 15f,33f,1f,-1f, 1,98, Result=true)]
		public bool IsMasked_WithProjection_Test(int width, int height, float x, float y, float projectionLeft, float projectionBottom, float scaleX, float scaleY, params int[] pointsInMask)
		{
			bool[][] array = GetArray(width, height, true, pointsInMask);
			AGSMask mask = new AGSMask(array, null);
			Mock<AGSSquare> square = new Mock<AGSSquare> ();

			AGS.API.PointF bottomLeft = new AGS.API.PointF (projectionLeft, projectionBottom);
			AGS.API.PointF bottomRight = new AGS.API.PointF (projectionLeft + width * Math.Abs(scaleX), projectionBottom);
			AGS.API.PointF topLeft = new AGS.API.PointF (projectionLeft, projectionBottom + height * Math.Abs(scaleY));
			AGS.API.PointF topRight = new AGS.API.PointF (projectionLeft + width * Math.Abs(scaleX), projectionBottom + height * Math.Abs(scaleY));
			square.Setup(s => s.BottomLeft).Returns(bottomLeft);
			square.Setup(s => s.BottomRight).Returns(bottomRight);
			square.Setup(s => s.TopLeft).Returns(topLeft);
			square.Setup(s => s.TopRight).Returns(topRight);

			return mask.IsMasked(new AGS.API.PointF (x, y), square.Object, scaleX, scaleY);
		}

		[TestCase(100,100, 0,0, Result=true)]
		[TestCase(100,100, 1,1, Result=true)]
		[TestCase(100,100, 5,5, Result=false)]
		[TestCase(100,100, 99,99, Result=false)]

		[TestCase(100,100, 0,0, 1,1, Result=true)]
		[TestCase(100,100, 1,1, 1,1, Result=true)]
		[TestCase(100,100, 5,5, 1,1, Result=false)]
		[TestCase(100,100, 99,99, 1,1, Result=false)]

		[TestCase(100,100, 5,5, 5,5, Result=true)]
		[TestCase(100,100, 5,5, 5,5, 4,4, 7,7, Result=true)]
		[TestCase(100,100, 5,5, 6,6, 4,4, 7,7, Result=false)]
		public bool ApplyToMaskTest(int width, int height, int xToTest, int yToTest, params int[] pointsInMask)
		{
			bool[][] targetMask = GetArray(width, height, true, 0,0, 1,1, 2,2);
			bool[][] sourceMask = GetArray(width, height, true, pointsInMask);
			AGSMask mask = new AGSMask (sourceMask, null);
			mask.ApplyToMask(targetMask);

			return targetMask[xToTest][yToTest];
		}

		public static bool[][] GetArray(int width, int height, bool markAsTrue, params int[] pointsInMask)
		{
			bool[][] array = new bool[width][];

			for (int x = 0; x < width; x++)
			{
				array[x] = new bool[height];
				for (int y = 0; y < height; y++)
				{
					array[x][y] = !markAsTrue;
				}
			}

			for (int pointIndex = 0; pointIndex < pointsInMask.Length; pointIndex += 2)
			{
				int x = pointsInMask[pointIndex];
				int y = pointsInMask[pointIndex + 1];
				array[x][y] = markAsTrue;
			}
			return array;
		}
	}
}

