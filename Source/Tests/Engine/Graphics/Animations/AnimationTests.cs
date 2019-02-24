using NUnit.Framework;
using AGS.API;
using AGS.Engine;
using Moq;

namespace Tests
{
	[TestFixture]
	public class AnimationTests
	{
		[TestCase(LoopingStyle.Forwards, 0, false)]
		[TestCase(LoopingStyle.ForwardsBackwards, 0, false)]
		[TestCase(LoopingStyle.Backwards, 4, true)]
		[TestCase(LoopingStyle.BackwardsForwards, 4, true)]
		public void SetupAnimationTest(LoopingStyle looping, int expectedFrame, bool expectedBackwards)
		{
            AGSAnimationConfiguration config = new AGSAnimationConfiguration { Looping = looping, DelayBetweenFrames = 0 };
			AGSAnimationState state = new AGSAnimationState ();
			AGSAnimation animation = new AGSAnimation (config, state);
			for (int i = 0; i < 5; i++)
			{
				animation.Frames.Add(getFrame(i * 2));
			}

			animation.Setup();

			Assert.AreEqual(expectedFrame, state.CurrentFrame);
			Assert.AreEqual(0, state.CurrentLoop);
			Assert.AreEqual(expectedBackwards, state.RunningBackwards);
			Assert.AreEqual(expectedFrame * 2, state.TimeToNextFrame);
		}

		[TestCase(LoopingStyle.Forwards, 0, 0, 0, false, 1, 0, false)]
		[TestCase(LoopingStyle.Forwards, 0, 0, 1, false, 1, 1, false)]
		[TestCase(LoopingStyle.Forwards, 0, 3, 1, false, 4, 1, false)]
		[TestCase(LoopingStyle.Forwards, 0, 4, 1, false, 0, 2, false)]

		[TestCase(LoopingStyle.ForwardsBackwards, 0, 0, 0, false, 1, 0, false)]
		[TestCase(LoopingStyle.ForwardsBackwards, 0, 0, 1, false, 1, 1, false)]
		[TestCase(LoopingStyle.ForwardsBackwards, 0, 3, 1, false, 4, 1, false)]
		[TestCase(LoopingStyle.ForwardsBackwards, 0, 4, 1, false, 4, 2, true)]

		[TestCase(LoopingStyle.ForwardsBackwards, 0, 0, 0, true, 0, 1, false)]
		[TestCase(LoopingStyle.ForwardsBackwards, 0, 0, 1, true, 0, 2, false)]
		[TestCase(LoopingStyle.ForwardsBackwards, 0, 3, 1, true, 2, 1, true)]
		[TestCase(LoopingStyle.ForwardsBackwards, 0, 4, 1, true, 3, 1, true)]

		[TestCase(LoopingStyle.Backwards, 0, 4, 0, true, 3, 0, true)]
		[TestCase(LoopingStyle.Backwards, 0, 4, 1, true, 3, 1, true)]
		[TestCase(LoopingStyle.Backwards, 0, 2, 1, true, 1, 1, true)]
		[TestCase(LoopingStyle.Backwards, 0, 0, 1, true, 4, 2, true)]

		[TestCase(LoopingStyle.BackwardsForwards, 0, 4, 0, true, 3, 0, true)]
		[TestCase(LoopingStyle.BackwardsForwards, 0, 4, 1, true, 3, 1, true)]
		[TestCase(LoopingStyle.BackwardsForwards, 0, 2, 1, true, 1, 1, true)]
		[TestCase(LoopingStyle.BackwardsForwards, 0, 0, 1, true, 0, 2, false)]

		[TestCase(LoopingStyle.BackwardsForwards, 0, 4, 0, false, 4, 1, true)]
		[TestCase(LoopingStyle.BackwardsForwards, 0, 4, 1, false, 4, 2, true)]
		[TestCase(LoopingStyle.BackwardsForwards, 0, 2, 1, false, 3, 1, false)]
		[TestCase(LoopingStyle.BackwardsForwards, 0, 0, 1, false, 1, 1, false)]
		public void NextFrameTest(LoopingStyle looping, int loops, int currentFrame, int currentLoop, bool runningBackwards, int expectedFrame, int expectedLoop, bool expectedBackwards)
		{
            AGSAnimationConfiguration config = new AGSAnimationConfiguration { Looping = looping, Loops = loops, DelayBetweenFrames = 0 };
			AGSAnimationState state = new AGSAnimationState {
				CurrentFrame = currentFrame,
				CurrentLoop = currentLoop,
				RunningBackwards = runningBackwards,
				TimeToNextFrame = currentFrame * 2
			};
			AGSAnimation animation = new AGSAnimation (config, state);
			for (int i = 0; i < 5; i++)
			{
				animation.Frames.Add(getFrame(i * 2));
			}

			animation.NextFrame();

			Assert.AreEqual(expectedFrame, state.CurrentFrame);
			Assert.AreEqual(expectedLoop, state.CurrentLoop);
			Assert.AreEqual(expectedBackwards, state.RunningBackwards);
			Assert.AreEqual(expectedFrame * 2, state.TimeToNextFrame);
		}

		private IAnimationFrame getFrame(int delay)
		{
			Mock<ISprite> sprite = new Mock<ISprite> ();
			return new AGSAnimationFrame (sprite.Object) { Delay = delay };
		}
	}
}

