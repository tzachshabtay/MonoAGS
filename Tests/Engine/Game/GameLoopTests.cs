using System;
using NUnit.Framework;
using Moq;
using API;
using Engine;
using System.Collections.Generic;
using System.Drawing;
using Autofac;

namespace Tests
{
	[TestFixture]
	public class GameLoopTests
	{
		private Mocks mocks;

		[SetUp]
		public void Init()
		{
			mocks = Mocks.Init();
		}

		[TearDown]
		public void Teardown()
		{
			mocks.Dispose();
		}

		[Test]
		public void EmptyRoomTest()
		{
			var room = mocks.Room();
			room.Setup(r => r.Background).Returns((IObject)null);
			var loop = getGameLoop();

			loop.Update();
		}

		[Test]
		public void Background_CounterDecreasedTest()
		{
			var animationState = mocks.AnimationState();
			animationState.Setup(a => a.TimeToNextFrame).Returns(5);
			var animation = mocks.Animation();

			var loop = getGameLoop();
			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = 4, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Never);
		}

		[Test]
		public void Background_NextFrameTest()
		{
			var animationState = mocks.AnimationState();
			int frame = 0;
			animationState.Setup(a => a.TimeToNextFrame).Returns(() => frame);
			animationState.SetupSet(a => a.TimeToNextFrame = It.IsAny<int>()).Callback<int>(i => frame = i);
			var animation = mocks.Animation();
			var loop = getGameLoop();

			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = -1, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Once());
		}

		[Test]
		public void Object_CounterDecreasedTest()
		{
			mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			mocks.Room().Setup(r => r.Objects).Returns(new List<IObject> { mocks.Object().Object });
			var animationState = mocks.AnimationState();
			animationState.Setup(a => a.TimeToNextFrame).Returns(5);
			var animation = mocks.Animation();

			var loop = getGameLoop();
			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = 4, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Never);
		}

		[Test]
		public void InvisibleObject_CounterNotDecreasedTest()
		{
			mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			mocks.Room().Setup(r => r.Objects).Returns(new List<IObject> { mocks.Object().Object });
			mocks.Object().Setup(o => o.Visible).Returns(false);
			var animationState = mocks.AnimationState();
			animationState.Setup(a => a.TimeToNextFrame).Returns(5);
			var animation = mocks.Animation();

			var loop = getGameLoop();
			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = 4, Times.Never());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Never());
			animation.Verify(a => a.NextFrame(), Times.Never());
		}

		[Test]
		public void PlayerInNoPlayerRoom_CounterNotDecreasedTest()
		{
			mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			mocks.Room().Setup(r => r.Objects).Returns(new List<IObject> { mocks.Character().Object });
			mocks.Room().Setup(r => r.ShowPlayer).Returns(false);
			var animationState = mocks.AnimationState();
			animationState.Setup(a => a.TimeToNextFrame).Returns(5);
			var animation = mocks.Animation();

			var loop = getGameLoop();
			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = 4, Times.Never());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Never());
			animation.Verify(a => a.NextFrame(), Times.Never());
		}

		[Test]
		public void ObjectInNoPlayerRoom_CounterDecreasedTest()
		{
			mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			mocks.Room().Setup(r => r.Objects).Returns(new List<IObject> { mocks.Object().Object });
			mocks.Room().Setup(r => r.ShowPlayer).Returns(false);
			var animationState = mocks.AnimationState();
			animationState.Setup(a => a.TimeToNextFrame).Returns(5);
			var animation = mocks.Animation();

			var loop = getGameLoop();
			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = 4, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Never());
		}

		[Test]
		public void Object_NextFrameTest()
		{
			mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			mocks.Room().Setup(r => r.Objects).Returns(new List<IObject> { mocks.Object().Object });
			var animationState = mocks.AnimationState();
			int frame = 0;
			animationState.Setup(a => a.TimeToNextFrame).Returns(() => frame);
			animationState.SetupSet(a => a.TimeToNextFrame = It.IsAny<int>()).Callback<int>(i => frame = i);
			var animation = mocks.Animation();
			var loop = getGameLoop();

			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = -1, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Once());
		}

		[Test]
		public void InvisibleObject_NotNextFrameTest()
		{
			mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			mocks.Room().Setup(r => r.Objects).Returns(new List<IObject> { mocks.Object().Object });
			mocks.Object().Setup(o => o.Visible).Returns(false);
			var animationState = mocks.AnimationState();
			int frame = 0;
			animationState.Setup(a => a.TimeToNextFrame).Returns(() => frame);
			animationState.SetupSet(a => a.TimeToNextFrame = It.IsAny<int>()).Callback<int>(i => frame = i);
			var animation = mocks.Animation();
			var loop = getGameLoop();

			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = -1, Times.Never());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Never());
			animation.Verify(a => a.NextFrame(), Times.Never());
		}

		[Test]
		public void PlayerInNoPlayerRoom_NotNextFrameTest()
		{
			mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			mocks.Room().Setup(r => r.Objects).Returns(new List<IObject> { mocks.Character().Object });
			var animationState = mocks.AnimationState();
			int frame = 0;
			animationState.Setup(a => a.TimeToNextFrame).Returns(() => frame);
			animationState.SetupSet(a => a.TimeToNextFrame = It.IsAny<int>()).Callback<int>(i => frame = i);
			var animation = mocks.Animation();
			var loop = getGameLoop();

			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = -1, Times.Never());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Never());
			animation.Verify(a => a.NextFrame(), Times.Never());
		}

		[Test]
		public void ObjectInNoPlayerRoom_NextFrameTest()
		{
			mocks.Room().Setup(r => r.Background).Returns((IObject)null);
			mocks.Room().Setup(r => r.Objects).Returns(new List<IObject> { mocks.Object().Object });
			var animationState = mocks.AnimationState();
			int frame = 0;
			animationState.Setup(a => a.TimeToNextFrame).Returns(() => frame);
			animationState.SetupSet(a => a.TimeToNextFrame = It.IsAny<int>()).Callback<int>(i => frame = i);
			var animation = mocks.Animation();
			var loop = getGameLoop();

			loop.Update();

			animationState.VerifySet(a => a.TimeToNextFrame = -1, Times.Once());
			animationState.VerifySet(a => a.TimeToNextFrame = It.IsAny<int>(), Times.Once());
			animation.Verify(a => a.NextFrame(), Times.Once());
		}

		[Test]
		public void NoViewportFollower_ViewportStaysTest()
		{
			mocks.Viewport().Setup(v => v.Follower).Returns((IFollower)null);
			var loop = getGameLoop();

			loop.Update();

			mocks.Viewport().VerifySet(v => v.X = It.IsAny<float>(), Times.Never());
			mocks.Viewport().VerifySet(v => v.Y = It.IsAny<float>(), Times.Never());
		}

		[Test]
		public void ViewportFollower_ViewportMovesTest()
		{
			Mock<IFollower> follower = new Mock<IFollower> ();
			IPoint sourcePoint = new AGSPoint (15f, 25f);
			IPoint targetPoint = new AGSPoint (55f, 65f);
			follower.Setup(f => f.Follow(sourcePoint, It.IsAny<Size>(), It.IsAny<Size>())).Returns(targetPoint);
			mocks.Viewport().Setup(v => v.X).Returns(sourcePoint.X);
			mocks.Viewport().Setup(v => v.Y).Returns(sourcePoint.Y);
			mocks.Viewport().Setup(v => v.Follower).Returns(follower.Object);
			var loop = getGameLoop();

			loop.Update();

			mocks.Viewport().VerifySet(v => v.X = It.IsAny<float>(), Times.Once());
			mocks.Viewport().VerifySet(v => v.Y = It.IsAny<float>(), Times.Once());
			mocks.Viewport().VerifySet(v => v.X = targetPoint.X, Times.Once());
			mocks.Viewport().VerifySet(v => v.Y = targetPoint.Y, Times.Once());
		}

		private AGSGameLoop getGameLoop()
		{
			return mocks.Create<AGSGameLoop>(new TypedParameter (typeof(Size), new Size (320, 200)));
		}
	}
}

