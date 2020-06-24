using NUnit.Framework;
using AGS.API;

namespace Tests
{
	[TestFixture ()]
	public class SquareTests
	{
		[TestCase(0f,0f, 10f,0f, 0f,10f, 10f,10f, 5f,5f, Result=true)]
		[TestCase(0f,0f, 10f,0f, 0f,10f, 10f,10f, 15f,15f, Result=false)]
		[TestCase(0f,0f, 10f,0f, 0f,10f, 10f,10f, 0f,0f, Result=true)]
		[TestCase(0f,0f, 10f,0f, 0f,10f, 10f,10f, 10f,10f, Result=true)]
		[TestCase(0f,0f, 10f,0f, 0f,10f, 10f,10f, 11f,11f, Result=false)]

		[TestCase(0f,0f, 10f,-10f, 10f,10f, 20f,0f, 10f,0f, Result=true)]
		[TestCase(0f,0f, 10f,-10f, 10f,10f, 20f,0f, 9f,9f, Result=true)]
		[TestCase(0f,0f, 10f,-10f, 10f,10f, 20f,0f, 9f,-9f, Result=true)]
		[TestCase(0f,0f, 10f,-10f, 10f,10f, 20f,0f, 9f,-11f, Result=false)]
		public bool InsideSquareTest (float ax, float ay, float bx, float @by, 
			float cx, float cy, float dx, float dy, float px, float py)
		{
            AGSBoundingBox square = new AGSBoundingBox (new Vector2 (ax, ay), new Vector2 (bx, @by),
				new Vector2 (cx, cy), new Vector2 (dx, dy));
			return square.Contains (new Vector2 (px, py));
		}
	}
}

