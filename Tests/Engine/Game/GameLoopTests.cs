using System;
using NUnit.Framework;
using Moq;
using AGS.API;
using AGS.Engine;
using System.Collections.Generic;
using System.Drawing;
using Autofac;

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
		public void EmptyRoomTest()
		{
			var room = _mocks.Room();
			room.Setup(r => r.Background).Returns((IObject)null);
			var loop = getGameLoop();

			loop.Update();
		}

		[Test]
		public void Background_CounterDecreasedTest()
		{
			var animationState = _mocks.AnimationState();
			animationState.Setup(a => a.TimeToNextFrame).Returns(5);
			var animation = _mocks.Animation();

			var loop = getGameLoop();
			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = 4, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Never);
		}

		[Test]
		public void Background_NextFrameTest()
		{
			var animationState = _mocks.AnimationState();
			int frame = 0;
			animationState.Setup(a => a.TimeToNextFrame).Returns(() => frame);
			animationState.SetupSet(a => a.TimeToNextFrame = It.IsAny<int>()).Callback<int>(i => frame = i);
			var animation = _mocks.Animation();
			var loop = getGameLoop();

			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = -1, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Once());
		}

		[Test]
		public void Object_CounterDecreasedTest()
		{
			_mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			_mocks.Room().Setup(r => r.Objects).Returns(new AGSConcurrentHashSet<IObject> { _mocks.Object().Object });
			var animationState = _mocks.AnimationState();
			animationState.Setup(a => a.TimeToNextFrame).Returns(5);
			var animation = _mocks.Animation();

			var loop = getGameLoop();
			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = 4, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Never);
		}

		[Test]
		public void InvisibleObject_CounterNotDecreasedTest()
		{
			_mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			_mocks.Room().Setup(r => r.Objects).Returns(new AGSConcurrentHashSet<IObject> { _mocks.Object().Object });
			_mocks.Object().Setup(o => o.Visible).Returns(false);
			var animationState = _mocks.AnimationState();
			animationState.Setup(a => a.TimeToNextFrame).Returns(5);
			var animation = _mocks.Animation();

			var loop = getGameLoop();
			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = 4, Times.Never());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Never());
			animation.Verify(a => a.NextFrame(), Times.Never());
		}

		[Test]
		public void PlayerInNoPlayerRoom_CounterNotDecreasedTest()
		{
			_mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			_mocks.Room().Setup(r => r.Objects).Returns(new AGSConcurrentHashSet<IObject> { _mocks.Character().Object });
			_mocks.Room().Setup(r => r.ShowPlayer).Returns(false);
			var animationState = _mocks.AnimationState();
			animationState.Setup(a => a.TimeToNextFrame).Returns(5);
			var animation = _mocks.Animation();

			var loop = getGameLoop();
			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = 4, Times.Never());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Never());
			animation.Verify(a => a.NextFrame(), Times.Never());
		}

		[Test]
		public void ObjectInNoPlayerRoom_CounterDecreasedTest()
		{
			_mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			_mocks.Room().Setup(r => r.Objects).Returns(new AGSConcurrentHashSet<IObject> { _mocks.Object().Object });
			_mocks.Room().Setup(r => r.ShowPlayer).Returns(false);
			var animationState = _mocks.AnimationState();
			animationState.Setup(a => a.TimeToNextFrame).Returns(5);
			var animation = _mocks.Animation();

			var loop = getGameLoop();
			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = 4, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Never());
		}

		[Test]
		public void Object_NextFrameTest()
		{
			_mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			_mocks.Room().Setup(r => r.Objects).Returns(new AGSConcurrentHashSet<IObject> { _mocks.Object().Object });
			var animationState = _mocks.AnimationState();
			int frame = 0;
			animationState.Setup(a => a.TimeToNextFrame).Returns(() => frame);
			animationState.SetupSet(a => a.TimeToNextFrame = It.IsAny<int>()).Callback<int>(i => frame = i);
			var animation = _mocks.Animation();
			var loop = getGameLoop();

			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = -1, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Once());
		}

		[Test]
		public void InvisibleObject_NotNextFrameTest()
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

			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = -1, Times.Never());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Never());
			animation.Verify(a => a.NextFrame(), Times.Never());
		}

		[Test]
		public void PlayerInNoPlayerRoom_NotNextFrameTest()
		{
			_mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			_mocks.Room().Setup(r => r.Objects).Returns(new AGSConcurrentHashSet<IObject> { _mocks.Character().Object });
			var animationState = _mocks.AnimationState();
			int frame = 0;
			animationState.Setup(a => a.TimeToNextFrame).Returns(() => frame);
			animationState.SetupSet(a => a.TimeToNextFrame = It.IsAny<int>()).Callback<int>(i => frame = i);
			var animation = _mocks.Animation();
			var loop = getGameLoop();

			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = -1, Times.Never());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Never());
			animation.Verify(a => a.NextFrame(), Times.Never());
		}

		[Test]
		public void ObjectInNoPlayerRoom_NextFrameTest()
		{
			_mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			_mocks.Room().Setup(r => r.Objects).Returns(new AGSConcurrentHashSet<IObject> { _mocks.Object().Object });
			var animationState = _mocks.AnimationState();
			int frame = 0;
			animationState.Setup(a => a.TimeToNextFrame).Returns(() => frame);
			animationState.SetupSet(a => a.TimeToNextFrame = It.IsAny<int>()).Callback<int>(i => frame = i);
			var animation = _mocks.Animation();
			var loop = getGameLoop();

			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = -1, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Once());
		}

		[Test]
		public void NoViewportFollower_ViewportStaysTest()
		{
			_mocks.Viewport().Setup(v => v.Follower).Returns((IFollower)null);
			var loop = getGameLoop();

			loop.Update();

			_mocks.Viewport().VerifySet(v => v.X = It.IsAny<float>(), Times.Never());
			_mocks.Viewport().VerifySet(v => v.Y = It.IsAny<float>(), Times.Never());
		}

		[Test]
		public void ViewportFollower_ViewportMovesTest()
		{
			Mock<IFollower> follower = new Mock<IFollower> ();
			IPoint sourcePoint = new AGSPoint (15f, 25f);
			IPoint targetPoint = new AGSPoint (55f, 65f);
			follower.Setup(f => f.Follow(sourcePoint, It.IsAny<Size>(), It.IsAny<Size>(),
				It.IsAny<bool>())).Returns(targetPoint);
			_mocks.Viewport().Setup(v => v.X).Returns(sourcePoint.X);
			_mocks.Viewport().Setup(v => v.Y).Returns(sourcePoint.Y);
			_mocks.Viewport().Setup(v => v.Follower).Returns(follower.Object);
			var loop = getGameLoop();

			loop.Update();

			_mocks.Viewport().VerifySet(v => v.X = It.IsAny<float>(), Times.Once());
			_mocks.Viewport().VerifySet(v => v.Y = It.IsAny<float>(), Times.Once());
			_mocks.Viewport().VerifySet(v => v.X = targetPoint.X, Times.Once());
			_mocks.Viewport().VerifySet(v => v.Y = targetPoint.Y, Times.Once());
		}

		private AGSGameLoop getGameLoop()
		{
			return _mocks.Create<AGSGameLoop>(new TypedParameter (typeof(Size), new Size (320, 200)));
		}
	}
}

