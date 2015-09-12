using NUnit.Framework;
using System;
using Engine;

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
			AGSSquare square = new AGSSquare (new AGSPoint (ax, ay), new AGSPoint (bx, @by),
				                   new AGSPoint (cx, cy), new AGSPoint (dx, dy));
			return square.Contains (new AGSPoint (px, py));
		}
	}
}

