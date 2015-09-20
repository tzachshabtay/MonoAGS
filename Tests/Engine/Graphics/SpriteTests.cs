using System;
using NUnit.Framework;
using AGS.Engine;
using AGS.API;
using Moq;
using System.Drawing;

namespace Tests
{
	[TestFixture]
	public class SpriteTests
	{
		private Mocks _mocks;

		[SetUp]
		public void Init()
		{
			_mocks = Mocks.Init();
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
			_mocks.Image().Setup(i => i.Width).Returns(imageWidth);
			_mocks.Image().Setup(i => i.Height).Returns(imageHeight);

			AGSSprite sprite = createSprite();
			if (imageBeforeScale)
			{
				sprite.Image = _mocks.Image().Object;
				sprite.ScaleBy(scaleX, scaleY);
			}
			else
			{
				sprite.Image = new Mock<IImage> ().Object;
				sprite.ScaleBy(scaleX, scaleY);
				sprite.Image = _mocks.Image().Object;
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
			_mocks.Image().Setup(i => i.Width).Returns(imageWidth);
			_mocks.Image().Setup(i => i.Height).Returns(imageHeight);

			AGSSprite sprite = createSprite();
			sprite.Image = _mocks.Image().Object;
            sprite.ScaleTo(toWidth, toHeight);
			
			Assert.AreEqual(toWidth, sprite.Width);
			Assert.AreEqual(toHeight, sprite.Height);
			Assert.AreEqual(expectedScaleX, sprite.ScaleX);
			Assert.AreEqual(expectedScaleY, sprite.ScaleY);

			testResetScale(imageWidth, imageHeight, sprite);
		}

		[Test]
		public void PerformingPixelPerfectTest()
		{
			Mock<IMask> mask = new Mock<IMask> ();
			Bitmap bitmap = new Bitmap (1, 1);
			_mocks.Image().Setup(i => i.OriginalBitmap).Returns(bitmap);
			_mocks.MaskLoader().Setup(m => m.Load(bitmap, false, null, null)).Returns(mask.Object);
			AGSSprite sprite = createSprite();
			sprite.Image = _mocks.Image().Object;

			sprite.PixelPerfect(true);

			Assert.AreEqual(mask.Object, sprite.PixelPerfectHitTestArea.Mask);
			Assert.IsTrue(sprite.PixelPerfectHitTestArea.Enabled);
		}

		[Test]
		public void PerformingPixelPerfect_OnlyOnce_Test()
		{
			_mocks.MaskLoader().Setup(m => m.Load((Bitmap)null, false, null, null)).Returns((IMask)null);
			AGSSprite sprite = createSprite();
			sprite.Image = _mocks.Image().Object;

			sprite.PixelPerfect(true);

			_mocks.MaskLoader().Verify(m => m.Load((Bitmap)null, false, null, null), Times.Once());
		}

		[Test]
		public void DisablingPixelPerfect_WhenDisabled_Test()
		{
			AGSSprite sprite = createSprite();
			sprite.PixelPerfect(false);
			Assert.IsNull(sprite.PixelPerfectHitTestArea);
		}

		[Test]
		public void DisablingPixelPerfect_WhenEnabled_Test()
		{
			_mocks.MaskLoader().Setup(m => m.Load((Bitmap)null, false, null, null)).Returns((IMask)null);
			AGSSprite sprite = createSprite();
			sprite.Image = _mocks.Image().Object;

			sprite.PixelPerfect(true);
			sprite.PixelPerfect(false);

			Assert.IsNotNull(sprite.PixelPerfectHitTestArea);
			Assert.IsFalse(sprite.PixelPerfectHitTestArea.Enabled);
		}

		static void testResetScale(float imageWidth, float imageHeight, AGSSprite sprite)
		{
			sprite.ResetScale();
			Assert.AreEqual(1f, sprite.ScaleX);
			Assert.AreEqual(1f, sprite.ScaleY);
			Assert.AreEqual(imageWidth, sprite.Width);
			Assert.AreEqual(imageHeight, sprite.Height);
		}

		private AGSSprite createSprite()
		{
			AGSSprite sprite = new AGSSprite (_mocks.MaskLoader().Object);
			return sprite;
		}
	}
}

