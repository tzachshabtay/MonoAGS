using System;
using NUnit.Framework;
using AGS.Engine;
using AGS.API;

namespace Tests
{
	[TestFixture]
	public class CameraTests
	{
		private Mocks _mocks;

		[SetUp]
		public void Init()
		{
			_mocks = Mocks.Init();
		}

		[TearDown]
		public void Teardown()
		{
			_mocks.Dispose();
		}

		//Viewport should clamp to min
		[TestCase(0f, 0f, 500, 300, 30f, Result = 0)]
		[TestCase(0f, 50f, 500, 300, 30f, Result = 0)]
		[TestCase(0f, 100f, 500, 300, 30f, Result = 0)]

		//Viewport should clamp to max
		[TestCase(200f, 500f, 500, 300, 30f, Result = 200)]
		[TestCase(200f, 450f, 500, 300, 30f, Result = 200)]
		[TestCase(200f, 400f, 500, 300, 30f, Result = 200)]

		//Viewport is spot on, should stay at the same location
		[TestCase(50f, 200f, 500, 300, 30f, Result = 50)]

		//Viewport should move
		[TestCase(49.9f, 200f, 500, 300, 30f, Result = 50)]
		[TestCase(40f, 200f, 500, 300, 30f, Result = 43)]
		[TestCase(40f, 200f, 500, 300, 20f, Result = 42)]
		[TestCase(60f, 200f, 500, 300, 30f, Result = 57)]
		[TestCase(60f, 200f, 500, 300, 20f, Result = 58)]
		public int Camera_FollowX_Test(float currentViewX, float targetPosX, 
			int roomWidth, int screenWidth, float speedX)
		{
			AGSCamera camera = new AGSCamera (speedX, 0f);
			_mocks.Object().Setup(o => o.X).Returns(targetPosX);
			_mocks.Object().Setup(o => o.Y).Returns(0f);
			Func<IObject> getTarget = () => _mocks.Object().Object;
			camera.Target = getTarget;
            AGSViewport viewport = new AGSViewport(new AGSDisplayListSettings(), camera, Mocks.GetResolver());
			viewport.X = currentViewX;
			viewport.Y = 0f;
            camera.Tick(viewport, new RectangleF (0f, 0f, roomWidth, 200), new Size (screenWidth, 200), false);
			return (int)viewport.X;
		}
	}
}

