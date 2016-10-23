using System;
using NUnit.Framework;
using AGS.Engine;
using AGS.API;
using Moq;
using System.Drawing;
using System.Collections.Generic;
using AGS.Engine.Desktop;

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
			foreach (IHasModelMatrix sprite in getImplementors())
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
			foreach (IHasModelMatrix sprite in getImplementors())
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
			foreach (var sprite in getPixelPerfectImplementors())
			{
				Mock<IMask> mask = new Mock<IMask> ();
				Bitmap bitmap = new Bitmap (1, 1);
                Mock<IGraphicsBackend> graphics = new Mock<IGraphicsBackend>();
                DesktopBitmap ibitmap = new DesktopBitmap (bitmap, graphics.Object);
				_mocks.Image().Setup(i => i.OriginalBitmap).Returns(ibitmap);
				_mocks.MaskLoader().Setup(m => m.Load(ibitmap, false, null, null)).Returns(mask.Object);

				sprite.Model.Image = _mocks.Image().Object;

				sprite.PixelPerfect.PixelPerfect(true);

				Assert.AreEqual(mask.Object, sprite.PixelPerfect.PixelPerfectHitTestArea.Mask);
				Assert.IsTrue(sprite.PixelPerfect.PixelPerfectHitTestArea.Enabled);
			}
		}

		[Test]
		public void PerformingPixelPerfect_OnlyOnce_Test()
		{
			int count = 0;
			foreach (var sprite in getPixelPerfectImplementors())
			{
				_mocks.MaskLoader().Setup(m => m.Load((IBitmap)null, false, null, null)).Returns((IMask)null);
				sprite.Model.Image = _mocks.Image().Object;

				sprite.PixelPerfect.PixelPerfect(true);
				count++;

				_mocks.MaskLoader().Verify(m => m.Load((IBitmap)null, false, null, null), Times.Exactly(count));
			}
		}

		[Test]
		public void DisablingPixelPerfect_WhenDisabled_Test()
		{
			foreach (var sprite in getPixelPerfectImplementors())
			{
                sprite.PixelPerfect.PixelPerfect(false);
				Assert.IsNull(sprite.PixelPerfect.PixelPerfectHitTestArea);
			}
		}

		[Test]
		public void DisablingPixelPerfect_WhenEnabled_Test()
		{
			foreach (var sprite in getPixelPerfectImplementors())
			{
				_mocks.MaskLoader().Setup(m => m.Load((IBitmap)null, false, null, null)).Returns((IMask)null);                
				sprite.Model.Image = _mocks.Image().Object;

				sprite.PixelPerfect.PixelPerfect(true);
				sprite.PixelPerfect.PixelPerfect(false);

				Assert.IsNotNull(sprite.PixelPerfect.PixelPerfectHitTestArea);
				Assert.IsFalse(sprite.PixelPerfect.PixelPerfectHitTestArea.Enabled);
			}
		}

		static void testResetScale(float imageWidth, float imageHeight, IHasModelMatrix sprite)
		{
			sprite.ResetScale();
			Assert.AreEqual(1f, sprite.ScaleX);
			Assert.AreEqual(1f, sprite.ScaleY);
			Assert.AreEqual(imageWidth, sprite.Width);
			Assert.AreEqual(imageHeight, sprite.Height);
		}

		private IEnumerable<IHasModelMatrix> getImplementors()
		{
			foreach (var sprite in ObjectTests.GetImplementors(_mocks, _mocks.GameState()))
			{
				//todo: Fix scaling for labels
				if (sprite is AGSLabel || sprite is AGSButton) continue;

				yield return sprite;
			}
            var resolver = ObjectTests.GetResolver();
            resolver.Build();
            yield return new AGSSprite (resolver, _mocks.MaskLoader().Object);
		}

        private IEnumerable<PixelPerfectImplementation> getPixelPerfectImplementors()
        {
            foreach (var sprite in getImplementors())
            {
                yield return new PixelPerfectImplementation(sprite);
            }
        }

        private class PixelPerfectImplementation
        {
            public PixelPerfectImplementation(IHasModelMatrix model)
            {
                Model = model;
                if (model is IPixelPerfectCollidable) PixelPerfect = (IPixelPerfectCollidable)model;
                else throw new InvalidOperationException("Missing pixel perfect implementation");
            }

            public IPixelPerfectCollidable PixelPerfect { get; private set; }

            public IHasModelMatrix Model { get; private set; }
        }
	}
}

