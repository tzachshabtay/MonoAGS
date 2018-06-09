using System;
using NUnit.Framework;
using AGS.Engine;
using AGS.API;

namespace Tests
{
	[TestFixture]
	public class LocationTests
	{
		[TestCase(0f, null, Result=0f)]
		[TestCase(0f, 0f, Result=0f)]
		[TestCase(0f, 1f, Result=1f)]
		public float Constructor_GetZ_Test(float y, float? z)
		{
            Position location = (15f, y, z);
			Assert.AreEqual(15f, location.X);
			Assert.AreEqual(y, location.Y);
			return location.Z;
		}
	}
}

