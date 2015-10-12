using System;
using NUnit.Framework;
using AGS.Engine;
using AGS.API;
using Moq;
using System.Drawing;
using System.Collections.Generic;

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
			foreach (ISprite sprite in getImplementors())
			{
				bool isSprite = sprite.GetType() == typeof(AGSSprite);
				_mocks.Image().Setup(i => i.Width).Returns(imageWidth);
				_mocks.Image().Setup(i => i.Height).Returns(imageHeight);

				if (imageBeforeScale || !isSprite)
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

				Assert.AreEqual(expectedWidth, sprite.Width, "Width doesn' match for " + sprite.GetType().Name);
				Assert.AreEqual(expectedHeight, sprite.Height, "Height doesn't match for " + sprite.GetType().Name);
				Assert.AreEqual(scaleX, sprite.ScaleX, "ScaleX doesn't match for " + sprite.GetType().Name);
				Assert.AreEqual(scaleY, sprite.ScaleY, "ScaleY doesn't match for " + sprite.GetType().Name);

				testResetScale(imageWidth, imageHeight, sprite);
			}
		}

		[TestCase(200f,100f, 1f,1f, 200f,100f)]
		[TestCase(200f,100f, 2f,1f, 400f,100f)]
		[TestCase(200f,100f, 0.5f,2f, 100f,200f)]
		public void ScaleToTest(float imageWidth, float imageHeight, float expectedScaleX, float expectedScaleY, 
			float toWidth, float toHeight)
		{
			foreach (ISprite sprite in getImplementors())
			{
				_mocks.Image().Setup(i => i.Width).Returns(imageWidth);
				_mocks.Image().Setup(i => i.Height).Returns(imageHeight);

				sprite.Image = _mocks.Image().Object;
				sprite.ScaleTo(toWidth, toHeight);
			
				Assert.AreEqual(toWidth, sprite.Width, "Width doesn' match for " + sprite.GetType().Name);
				Assert.AreEqual(toHeight, sprite.Height, "Height doesn't match for " + sprite.GetType().Name);
				Assert.AreEqual(expectedScaleX, sprite.ScaleX, "ScaleX doesn't match for " + sprite.GetType().Name);
				Assert.AreEqual(expectedScaleY, sprite.ScaleY, "ScaleY doesn't match for " + sprite.GetType().Name);

				testResetScale(imageWidth, imageHeight, sprite);
			}
		}

		[Test]
		public void PerformingPixelPerfectTest()
		{
			foreach (ISprite sprite in getImplementors())
			{
				Mock<IMask> mask = new Mock<IMask> ();
				Bitmap bitmap = new Bitmap (1, 1);
				_mocks.Image().Setup(i => i.OriginalBitmap).Returns(bitmap);
				_mocks.MaskLoader().Setup(m => m.Load(bitmap, false, null, null)).Returns(mask.Object);

				sprite.Image = _mocks.Image().Object;

				sprite.PixelPerfect(true);

				Assert.AreEqual(mask.Object, sprite.PixelPerfectHitTestArea.Mask);
				Assert.IsTrue(sprite.PixelPerfectHitTestArea.Enabled);
			}
		}

		[Test]
		public void PerformingPixelPerfect_OnlyOnce_Test()
		{
			int count = 0;
			foreach (ISprite sprite in getImplementors())
			{
				_mocks.MaskLoader().Setup(m => m.Load((Bitmap)null, false, null, null)).Returns((IMask)null);
				sprite.Image = _mocks.Image().Object;

				sprite.PixelPerfect(true);
				count++;

				_mocks.MaskLoader().Verify(m => m.Load((Bitmap)null, false, null, null), Times.Exactly(count));
			}
		}

		[Test]
		public void DisablingPixelPerfect_WhenDisabled_Test()
		{
			foreach (ISprite sprite in getImplementors())
			{
				sprite.PixelPerfect(false);
				Assert.IsNull(sprite.PixelPerfectHitTestArea);
			}
		}

		[Test]
		public void DisablingPixelPerfect_WhenEnabled_Test()
		{
			foreach (ISprite sprite in getImplementors())
			{
				_mocks.MaskLoader().Setup(m => m.Load((Bitmap)null, false, null, null)).Returns((IMask)null);
				sprite.Image = _mocks.Image().Object;

				sprite.PixelPerfect(true);
				sprite.PixelPerfect(false);

				Assert.IsNotNull(sprite.PixelPerfectHitTestArea);
				Assert.IsFalse(sprite.PixelPerfectHitTestArea.Enabled);
			}
		}

		static void testResetScale(float imageWidth, float imageHeight, ISprite sprite)
		{
			sprite.ResetScale();
			Assert.AreEqual(1f, sprite.ScaleX);
			Assert.AreEqual(1f, sprite.ScaleY);
			Assert.AreEqual(imageWidth, sprite.Width);
			Assert.AreEqual(imageHeight, sprite.Height);
		}

		private IEnumerable<ISprite> getImplementors()
		{
			foreach (var sprite in ObjectTests.GetImplementors(_mocks, _mocks.GameState().Object))
			{
				//todo: Fix scaling for labels
				if (sprite is AGSLabel || sprite is AGSButton) continue;

				yield return sprite;
			}
			yield return new AGSSprite (_mocks.MaskLoader().Object);
		}			
	}
}

