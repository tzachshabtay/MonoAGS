using System;
using NUnit.Framework;
using AGS.API;

namespace Tests
{
	[TestFixture]
	public class ColorTests
	{
		[TestCase(0,0f,0f,0,0,0)] //black
		[TestCase(0,0f,1f,255,255,255)] //white
		[TestCase(0,1f,0.5f,255,0,0)] //red
		[TestCase(120,1f,0.5f,0,255,0)] //green
		[TestCase(240,1f,0.5f,0,0,255)] //blue
		[TestCase(60,1f,0.5f,255,255,0)] //yellow
		//[TestCase(60,1f,0.25f,128,128,0)] //olive -> This fails: R returns as 127 instead of 128 due to midpoint rounding to even on PCL
		[TestCase(359,1f,0.75f,255,128,130)]
		public void HslTest(int h, float s, float l, byte r, byte g, byte b)
		{
			Color color = Color.FromHsla(h, s, l, 100);
			Assert.AreEqual(r, color.R);
			Assert.AreEqual(g, color.G);
			Assert.AreEqual(b, color.B);
			Assert.AreEqual(h, color.GetHue());
			Assert.IsTrue(Math.Abs(color.GetSaturation() - s) < 0.01f);
			Assert.IsTrue(Math.Abs(color.GetLightness() - l) < 0.01f);
		}
	}
}

