using NUnit.Framework;
using AGS.API;

namespace Tests
{
	[TestFixture]
	public class MathUtilsTests
	{
		[TestCase(1, Result= 1)]
		[TestCase(2, Result= 2)]
		[TestCase(3, Result= 4)]
		[TestCase(4, Result= 4)]
		[TestCase(13, Result= 16)]
		[TestCase(33, Result= 64)]
		[TestCase(1000, Result= 1024)]
		[TestCase(1024, Result= 1024)]
		public int NextPowerOf2Test(int num)
		{
			return MathUtils.GetNextPowerOf2(num);
		}

		[TestCase(1, Result= true)]
		[TestCase(2, Result= true)]
		[TestCase(3, Result= false)]
		[TestCase(4, Result= true)]
		[TestCase(13, Result= false)]
		[TestCase(33, Result= false)]
		[TestCase(1000, Result= false)]
		[TestCase(1024, Result= true)]
		public bool IsPowerOf2Test(int num)
		{
			return MathUtils.IsPowerOf2(num);
		}

        [TestCase(1f, 1f, true)]
        [TestCase(1f, 0f, false)]
        [TestCase(-1f, -1f, true)]
        [TestCase(-1f, 1f, false)]
        [TestCase(float.NaN, float.NaN, true)]
        [TestCase(float.NaN, 0f, false)]
        [TestCase(-1f, -1.000000001f, true)]
        [TestCase(float.MinValue, float.MinValue, true)]
        [TestCase(float.MaxValue, float.MaxValue, true)]
        [TestCase(float.MaxValue, float.MinValue, false)]
        public void FloatEqualsTest(float x, float y, bool shouldEqual)
        {
            Assert.AreEqual(shouldEqual, MathUtils.FloatEquals(x, y));
        }
    }
}

