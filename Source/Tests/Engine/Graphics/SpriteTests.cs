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

		[TestCase(200f,100f, 1f,1f, 200f,100f)]
		[TestCase(200f,100f, 2f,1f, 400f,100f)]
		[TestCase(200f,100f, 0.5f,2f, 100f,200f)]
		public void ScaleByTest(float imageWidth, float imageHeight, float scaleX, float scaleY, 
			float expectedWidth, float expectedHeight)
		{
			foreach (IHasModelMatrix sprite in getImplementors())
			{
				_mocks.Image().Setup(i => i.Width).Returns(imageWidth);
				_mocks.Image().Setup(i => i.Height).Returns(imageHeight);

				sprite.Image = _mocks.Image().Object;
                sprite.Scale = new AGS.API.PointF(scaleX, scaleY);

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
				_mocks.MaskLoader().Setup(m => m.Load(It.IsAny<string>(), ibitmap, false, null, null)).Returns(mask.Object);

				sprite.Model.Image = _mocks.Image().Object;

                sprite.PixelPerfect.IsPixelPerfect = true;

				Assert.AreEqual(mask.Object, sprite.PixelPerfect.PixelPerfectHitTestArea.Mask);
				Assert.IsTrue(sprite.PixelPerfect.PixelPerfectHitTestArea.Enabled);
			}
		}

		[Test]
		public void PerformingPixelPerfect_OnlyOnce_Test()
		{
			foreach (var sprite in getPixelPerfectImplementors())
			{
				Mock<IMask> mask = new Mock<IMask>();
                Mock<IBitmap> bitmap = new Mock<IBitmap>();
                _mocks.MaskLoader().Setup(m => m.Load(It.IsAny<string>(), bitmap.Object, false, null, null)).Returns(mask.Object);
				sprite.Model.Image = _mocks.Image().Object;
                _mocks.Image().Setup(i => i.OriginalBitmap).Returns(bitmap.Object);

                sprite.PixelPerfect.IsPixelPerfect = true;
                var area = sprite.PixelPerfect.PixelPerfectHitTestArea;
                Assert.IsNotNull(area);

                _mocks.MaskLoader().Verify(m => m.Load(It.IsAny<string>(), bitmap.Object, false, null, null), Times.Once);
			}
		}

		[Test]
		public void DisablingPixelPerfect_WhenDisabled_Test()
		{
			foreach (var sprite in getPixelPerfectImplementors())
			{
                sprite.PixelPerfect.IsPixelPerfect = false;
                Assert.IsFalse(sprite.PixelPerfect.IsPixelPerfect);
			}
		}

		[Test]
		public void DisablingPixelPerfect_WhenEnabled_Test()
		{
			foreach (var sprite in getPixelPerfectImplementors())
			{
                Mock<IMask> mask = new Mock<IMask>();
                _mocks.MaskLoader().Setup(m => m.Load(It.IsAny<string>(), (IBitmap)null, false, null, null)).Returns(mask.Object);
				sprite.Model.Image = _mocks.Image().Object;
                _mocks.Image().Setup(i => i.OriginalBitmap).Returns(new Mock<IBitmap>().Object);

                sprite.PixelPerfect.IsPixelPerfect = true;
                sprite.PixelPerfect.IsPixelPerfect = false;

				Assert.IsNotNull(sprite.PixelPerfect.PixelPerfectHitTestArea);
                Assert.IsFalse(sprite.PixelPerfect.IsPixelPerfect);
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
                if (sprite is IPixelPerfectComponent) yield return new PixelPerfectImplementation(sprite);
            }
        }

        private class PixelPerfectImplementation
        {
            public PixelPerfectImplementation(IHasModelMatrix model)
            {
                Model = model;
                PixelPerfect = (IPixelPerfectComponent)model;
            }

            public IPixelPerfectComponent PixelPerfect { get; private set; }

            public IHasModelMatrix Model { get; private set; }
        }
	}
}