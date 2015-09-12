using System;
using NUnit.Framework;
using Engine;
using API;
using Moq;

namespace Tests
{
	[TestFixture]
	public class SpriteTests
	{
		private Mocks mocks;

		[SetUp]
		public void Init()
		{
			mocks = Mocks.Init();
		}

		[TestCase(200f,100f, 1f,1f, true, 200f,100f)]
		[TestCase(200f,100f, 1f,1f, false, 200f,100f)]

		[TestCase(200f,100f, 2f,1f, true, 400f,100f)]
		[TestCase(200f,100f, 2f,1f, false, 400f,100f)]

		[TestCase(200f,100f, 0.5f,2f, true, 100f,200f)]
		[TestCase(200f,100f, 0.5f,2f, false, 100f,200f)]
		public void ScaleByTest(float imageWidth, float imageHeight, float scaleX, float scaleY, 
			bool imageBeforeScale, float expectedWidth, float expectedHeight)
		{
			mocks.Image().Setup(i => i.Width).Returns(imageWidth);
			mocks.Image().Setup(i => i.Height).Returns(imageHeight);

			AGSSprite sprite = new AGSSprite ();
			if (imageBeforeScale)
			{
				sprite.Image = mocks.Image().Object;
				sprite.ScaleBy(scaleX, scaleY);
			}
			else
			{
				sprite.Image = new Mock<IImage> ().Object;
				sprite.ScaleBy(scaleX, scaleY);
				sprite.Image = mocks.Image().Object;
			}

			Assert.AreEqual(expectedWidth, sprite.Width);
			Assert.AreEqual(expectedHeight, sprite.Height);
			Assert.AreEqual(scaleX, sprite.ScaleX);
			Assert.AreEqual(scaleY, sprite.ScaleY);

			testResetScale(imageWidth, imageHeight, sprite);
		}

		[TestCase(200f,100f, 1f,1f, 200f,100f)]
		[TestCase(200f,100f, 2f,1f, 400f,100f)]
		[TestCase(200f,100f, 0.5f,2f, 100f,200f)]
		public void ScaleToTest(float imageWidth, float imageHeight, float expectedScaleX, float expectedScaleY, 
			float toWidth, float toHeight)
		{
			mocks.Image().Setup(i => i.Width).Returns(imageWidth);
			mocks.Image().Setup(i => i.Height).Returns(imageHeight);

			AGSSprite sprite = new AGSSprite ();
			sprite.Image = mocks.Image().Object;
            sprite.ScaleTo(toWidth, toHeight);
			
			Assert.AreEqual(toWidth, sprite.Width);
			Assert.AreEqual(toHeight, sprite.Height);
			Assert.AreEqual(expectedScaleX, sprite.ScaleX);
			Assert.AreEqual(expectedScaleY, sprite.ScaleY);

			testResetScale(imageWidth, imageHeight, sprite);
		}

		static void testResetScale(float imageWidth, float imageHeight, AGSSprite sprite)
		{
			sprite.ResetScale();
			Assert.AreEqual(1f, sprite.ScaleX);
			Assert.AreEqual(1f, sprite.ScaleY);
			Assert.AreEqual(imageWidth, sprite.Width);
			Assert.AreEqual(imageHeight, sprite.Height);
		}
	}
}

