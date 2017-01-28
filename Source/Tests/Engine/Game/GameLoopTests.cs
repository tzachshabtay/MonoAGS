using NUnit.Framework;
using Moq;
using AGS.API;
using AGS.Engine;
using Autofac;
using System.Threading.Tasks;

namespace Tests
{
	[TestFixture]
	public class GameLoopTests
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

		[Test]
		public async Task EmptyRoomTest()
		{
			var room = _mocks.Room();
			room.Setup(r => r.Background).Returns((IObject)null);
			var loop = getGameLoop();

			await loop.UpdateAsync();
		}

		[Test]
		public async Task Background_CounterDecreasedTest()
		{
			var animationState = _mocks.AnimationState();
			animationState.Setup(a => a.TimeToNextFrame).Returns(5);
			var animation = _mocks.Animation();

			var loop = getGameLoop();
			await loop.UpdateAsync();

			animationState.VerifySet(a => a.TimeToNextFrame = 4, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Never);
		}

		[Test]
		public async Task Background_NextFrameTest()
		{
			var animationState = _mocks.AnimationState();
			int frame = 0;
			animationState.Setup(a => a.TimeToNextFrame).Returns(() => frame);
			animationState.SetupSet(a => a.TimeToNextFrame = It.IsAny<int>()).Callback<int>(i => frame = i);
			var animation = _mocks.Animation();
			var loop = getGameLoop();

			await loop.UpdateAsync();

			animationState.VerifySet(a => a.TimeToNextFrame = -1, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Once());
		}

		[Test]
		public async Task Object_CounterDecreasedTest()
		{
			_mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			_mocks.Room().Setup(r => r.Objects).Returns(new AGSConcurrentHashSet<IObject> { _mocks.Object().Object });
			var animationState = _mocks.AnimationState();
			animationState.Setup(a => a.TimeToNextFrame).Returns(5);
			var animation = _mocks.Animation();

			var loop = getGameLoop();
			await loop.UpdateAsync();

			animationState.VerifySet(a => a.TimeToNextFrame = 4, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Never);
		}

		[Test]
		public async Task InvisibleObject_CounterNotDecreasedTest()
		{
			_mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			_mocks.Room().Setup(r => r.Objects).Returns(new AGSConcurrentHashSet<IObject> { _mocks.Object().Object });
			_mocks.Object().Setup(o => o.Visible).Returns(false);
			var animationState = _mocks.AnimationState();
			animationState.Setup(a => a.TimeToNextFrame).Returns(5);
			var animation = _mocks.Animation();

			var loop = getGameLoop();
			await loop.UpdateAsync();

			animationState.VerifySet(a => a.TimeToNextFrame = 4, Times.Never());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Never());
			animation.Verify(a => a.NextFrame(), Times.Never());
		}

		[Test]
		public async Task PlayerInNoPlayerRoom_CounterNotDecreasedTest()
		{
			_mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			_mocks.Room().Setup(r => r.Objects).Returns(new AGSConcurrentHashSet<IObject> { _mocks.Character().Object });
			_mocks.Room().Setup(r => r.ShowPlayer).Returns(false);
			var animationState = _mocks.AnimationState();
			animationState.Setup(a => a.TimeToNextFrame).Returns(5);
			var animation = _mocks.Animation();

			var loop = getGameLoop();
			await loop.UpdateAsync();

			animationState.VerifySet(a => a.TimeToNextFrame = 4, Times.Never());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Never());
			animation.Verify(a => a.NextFrame(), Times.Never());
		}

		[Test]
		public async Task ObjectInNoPlayerRoom_CounterDecreasedTest()
		{
			_mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			_mocks.Room().Setup(r => r.Objects).Returns(new AGSConcurrentHashSet<IObject> { _mocks.Object().Object });
			_mocks.Room().Setup(r => r.ShowPlayer).Returns(false);
			var animationState = _mocks.AnimationState();
			animationState.Setup(a => a.TimeToNextFrame).Returns(5);
			var animation = _mocks.Animation();

			var loop = getGameLoop();
			await loop.UpdateAsync();

			animationState.VerifySet(a => a.TimeToNextFrame = 4, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Never());
		}

		[Test]
		public async Task Object_NextFrameTest()
		{
			_mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			_mocks.Room().Setup(r => r.Objects).Returns(new AGSConcurrentHashSet<IObject> { _mocks.Object().Object });
			var animationState = _mocks.AnimationState();
			int frame = 0;
			animationState.Setup(a => a.TimeToNextFrame).Returns(() => frame);
			animationState.SetupSet(a => a.TimeToNextFrame = It.IsAny<int>()).Callback<int>(i => frame = i);
			var animation = _mocks.Animation();
			var loop = getGameLoop();

			await loop.UpdateAsync();

			animationState.VerifySet(a => a.TimeToNextFrame = -1, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Once());
		}

		[Test]
		public async Task InvisibleObject_NotNextFrameTest()
		{
			_mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			_mocks.Room().Setup(r => r.Objects).Returns(new AGSConcurrentHashSet<IObject> { _mocks.Object().Object });
			_mocks.Object().Setup(o => o.Visible).Returns(false);
			var animationState = _mocks.AnimationState();
			int frame = 0;
			animationState.Setup(a => a.TimeToNextFrame).Returns(() => frame);
			animationState.SetupSet(a => a.TimeToNextFrame = It.IsAny<int>()).Callback<int>(i => frame = i);
			var animation = _mocks.Animation();
			var loop = getGameLoop();

			await loop.UpdateAsync();

			animationState.VerifySet(a => a.TimeToNextFrame = -1, Times.Never());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Never());
			animation.Verify(a => a.NextFrame(), Times.Never());
		}

		[Test]
		public async Task PlayerInNoPlayerRoom_NotNextFrameTest()
		{
			_mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			_mocks.Room().Setup(r => r.Objects).Returns(new AGSConcurrentHashSet<IObject> { _mocks.Character().Object });
			var animationState = _mocks.AnimationState();
			int frame = 0;
			animationState.Setup(a => a.TimeToNextFrame).Returns(() => frame);
			animationState.SetupSet(a => a.TimeToNextFrame = It.IsAny<int>()).Callback<int>(i => frame = i);
			var animation = _mocks.Animation();
			var loop = getGameLoop();

			await loop.UpdateAsync();

			animationState.VerifySet(a => a.TimeToNextFrame = -1, Times.Never());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Never());
			animation.Verify(a => a.NextFrame(), Times.Never());
		}

		[Test]
		public async Task ObjectInNoPlayerRoom_NextFrameTest()
		{
			_mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			_mocks.Room().Setup(r => r.Objects).Returns(new AGSConcurrentHashSet<IObject> { _mocks.Object().Object });
			var animationState = _mocks.AnimationState();
			int frame = 0;
			animationState.Setup(a => a.TimeToNextFrame).Returns(() => frame);
			animationState.SetupSet(a => a.TimeToNextFrame = It.IsAny<int>()).Callback<int>(i => frame = i);
			var animation = _mocks.Animation();
			var loop = getGameLoop();

			await loop.UpdateAsync();

			animationState.VerifySet(a => a.TimeToNextFrame = -1, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Once());
		}

		[Test]
		public async Task NoCamera_ViewportStaysTest()
		{
			_mocks.Viewport().Setup(v => v.Camera).Returns((ICamera)null);
			var loop = getGameLoop();

			await loop.UpdateAsync();

			_mocks.Viewport().VerifySet(v => v.X = It.IsAny<float>(), Times.Never());
			_mocks.Viewport().VerifySet(v => v.Y = It.IsAny<float>(), Times.Never());
		}

		[Test]
		public async Task Camera_ViewportMovesTest()
		{
			Mock<ICamera> camera = new Mock<ICamera> ();
			AGS.API.PointF sourcePoint = new AGS.API.PointF (15f, 25f);
			AGS.API.PointF targetPoint = new AGS.API.PointF (55f, 65f);
			camera.Setup(f => f.Tick(_mocks.Viewport().Object, It.IsAny<AGS.API.Size>(), It.IsAny<AGS.API.Size>(),
				It.IsAny<bool>())).Callback(() => 
				{_mocks.Viewport().Object.X = targetPoint.X; _mocks.Viewport().Object.Y = targetPoint.Y; });
			_mocks.Viewport().Setup(v => v.X).Returns(sourcePoint.X);
			_mocks.Viewport().Setup(v => v.Y).Returns(sourcePoint.Y);
			_mocks.Viewport().Setup(v => v.Camera).Returns(camera.Object);
			var loop = getGameLoop();

			await loop.UpdateAsync();

			_mocks.Viewport().VerifySet(v => v.X = It.IsAny<float>(), Times.Once());
			_mocks.Viewport().VerifySet(v => v.Y = It.IsAny<float>(), Times.Once());
			_mocks.Viewport().VerifySet(v => v.X = targetPoint.X, Times.Once());
			_mocks.Viewport().VerifySet(v => v.Y = targetPoint.Y, Times.Once());
		}

		private AGSGameLoop getGameLoop()
		{
			return _mocks.Create<AGSGameLoop>(new TypedParameter (typeof(AGS.API.Size), new AGS.API.Size (320, 200)));
		}
	}
}

