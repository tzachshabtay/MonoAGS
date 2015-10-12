using System;
using NUnit.Framework;
using AGS.API;
using System.Collections.Generic;
using AGS.Engine;
using Moq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Tests
{
	[TestFixture]
	public class AnimationContainerTests
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
		public void StartFirstAnimationTest()
		{
			foreach (var container in getImplementors())
			{
				Mock<IAnimation> animation = _mocks.Animation();
				container.StartAnimation(animation.Object);
				Assert.AreEqual(animation.Object, container.Animation, "Animation not changed for " + container.GetType().Name);
			}
		}

		[Test]
		public async Task AnimationCompletedTest()
		{
			foreach (var container in getImplementors())
			{
				TaskCompletionSource<AnimationCompletedEventArgs> tcs = new TaskCompletionSource<AnimationCompletedEventArgs> ();
				_mocks.AnimationState(true).Setup(a => a.OnAnimationCompleted).Returns(tcs);
				Mock<IAnimation> animation = _mocks.Animation(true);
				Task<AnimationCompletedEventArgs> task = Task.Run(() => container.Animate(animation.Object));
				tcs.SetResult(new AnimationCompletedEventArgs (true));
				var args = await animateWithTimeout(container, task);
				Assert.AreEqual(animation.Object, container.Animation, "Animation not changed for " + container.GetType().Name);
				Assert.IsTrue(args.CompletedSuccessfully, "Animation not completed for " + container.GetType().Name);
			}
		}

		[Test]
		public async Task AnimationAsyncCompletedTest()
		{
			foreach (var container in getImplementors())
			{
				TaskCompletionSource<AnimationCompletedEventArgs> tcs = new TaskCompletionSource<AnimationCompletedEventArgs> ();
				_mocks.AnimationState(true).Setup(a => a.OnAnimationCompleted).Returns(tcs);
				Mock<IAnimation> animation = _mocks.Animation(true);
				Task<AnimationCompletedEventArgs> task = container.AnimateAsync(animation.Object);
				tcs.SetResult(new AnimationCompletedEventArgs (true));
				var args = await animateWithTimeout(container, task);
				Assert.AreEqual(animation.Object, container.Animation, "Animation not changed for " + container.GetType().Name);
				Assert.IsTrue(args.CompletedSuccessfully, "Animation not completed for " + container.GetType().Name);
			}
		}

		[Test]
		public async Task AnimationCancelledTest()
		{
			foreach (var container in getImplementors())
			{
				Mock<IAnimation> animation = _mocks.Animation();
				Task<AnimationCompletedEventArgs> task = Task.Run(() => container.Animate(animation.Object));
				await Task.Delay(10);
				Mock<IAnimation> newAnimation = _mocks.Animation(true);
				container.StartAnimation(newAnimation.Object);
				var args = await animateWithTimeout(container, task);
				Assert.AreEqual(newAnimation.Object, container.Animation, "Animation not changed for " + container.GetType().Name);
				Assert.IsFalse(args.CompletedSuccessfully, "Animation not cancelled for " + container.GetType().Name);
			}
		}

		[Test]
		public async Task AnimationAsyncCancelledTest()
		{
			foreach (var container in getImplementors())
			{
				Mock<IAnimation> animation = _mocks.Animation();
				Task<AnimationCompletedEventArgs> task = container.AnimateAsync(animation.Object);
				await Task.Delay(10);
				Mock<IAnimation> newAnimation = _mocks.Animation(true);
				container.StartAnimation(newAnimation.Object);
				var args = await animateWithTimeout(container, task);
				Assert.AreEqual(newAnimation.Object, container.Animation, "Animation not changed for " + container.GetType().Name);
				Assert.IsFalse(args.CompletedSuccessfully, "Animation not cancelled for " + container.GetType().Name);
			}
		}

		private async Task<AnimationCompletedEventArgs> animateWithTimeout(IAnimationContainer container, Task<AnimationCompletedEventArgs> task)
		{
			Task<AnimationCompletedEventArgs> timeoutTask = Task.Run(async () =>
			{
				await Task.Delay(10000);
				return (AnimationCompletedEventArgs)null;
			});
			Task<AnimationCompletedEventArgs> result = await Task.WhenAny(task, timeoutTask);
			Assert.AreEqual(task, result, "Animation timeout for " + container.GetType().Name);
			return result.Result;
		}

		private IEnumerable<IAnimationContainer> getImplementors()
		{
			foreach (var container in ObjectTests.GetImplementors(_mocks, _mocks.GameState().Object))
			{
				yield return container;
			}
			yield return new AGSAnimationContainer (_mocks.Sprite().Object, null);
		}
	}
}

