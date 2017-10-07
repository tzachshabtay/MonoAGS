using System;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
    [PropertyFolder]
	public class AGSAnimation : IAnimation
	{
		public AGSAnimation (IAnimationConfiguration configuration, IAnimationState state, int estimatedNumberOfFrames = 8)
		{
			Configuration = configuration;
			State = state;
			Frames = new List<IAnimationFrame> (estimatedNumberOfFrames);
		}

		#region IAnimation implementation

		public void Setup()
		{
			int frame = 0;
			bool runningBackwards = false;
			switch (Configuration.Looping) 
			{
				case LoopingStyle.Backwards:
				case LoopingStyle.BackwardsForwards:
					frame = Frames.Count - 1;
					runningBackwards = true;
					break;
			}
					
			State.CurrentFrame = frame;
			State.CurrentLoop = 0;
			State.RunningBackwards = runningBackwards;
            State.TimeToNextFrame = Frames.Count == 0 ? -1 : getTimeForNextFrame(Frames[frame]);
		}

		public bool NextFrame ()
		{
			if (finishedAnimation) return false;

			int frame = State.CurrentFrame;
			bool endedLoop = false;
			bool runningBackwards = State.RunningBackwards;
			if (runningBackwards) 
			{
				frame--;
				endedLoop = frame < 0;
			} 
			else 
			{
				frame++;
				endedLoop = frame >= Frames.Count;
			}
			if (endedLoop) 
			{
				if (State.CurrentLoop >= Configuration.Loops && Configuration.Loops > 0) 
				{
					finishedAnimation = true;
					State.OnAnimationCompleted.TrySetResult (new AnimationCompletedEventArgs (true));
					return true;
				}

				switch (Configuration.Looping) 
				{
					case LoopingStyle.Backwards:
						frame = Frames.Count - 1;
						break;
					case LoopingStyle.Forwards:
						frame = 0;
						break;
					case LoopingStyle.BackwardsForwards:
					case LoopingStyle.ForwardsBackwards:
						runningBackwards = !runningBackwards;
						frame = runningBackwards ? Frames.Count - 1 : 0;
						break;
					default:
						throw new NotSupportedException (string.Format ("{0} is not a supported looping style", Configuration.Looping));
				}
				State.CurrentLoop++;
			}
			State.CurrentFrame = frame;
			State.RunningBackwards = runningBackwards;
			var animationFrame = Frames[frame];
            State.TimeToNextFrame = getTimeForNextFrame(animationFrame);
			var emitter = animationFrame.SoundEmitter;
			if (emitter != null) emitter.Play();
			return true;
		}

		public void FlipHorizontally()
		{
			foreach (var frame in Frames)
			{
				frame.Sprite.FlipHorizontally();
			}
		}

		public void FlipVertically()
		{
			foreach (var frame in Frames)
			{
				frame.Sprite.FlipVertically();
			}
		}

		public IAnimation Clone()
		{
			AGSAnimation clone = (AGSAnimation)MemberwiseClone();
			clone.Frames = new List<IAnimationFrame> (Frames.Count);
			foreach (var frame in Frames)
			{
				clone.Frames.Add(frame.Clone());
			}
			return clone;
		}

		public IList<IAnimationFrame> Frames { get; private set; }

		public IAnimationConfiguration Configuration { get; private set; }

		public IAnimationState State { get; private set; }

		public ISprite Sprite 
		{ 
			get 
			{ 
				if (State.CurrentFrame >= Frames.Count) return null;
				return Frames[State.CurrentFrame].Sprite; 
			} 
		}

        #endregion

        private int getTimeForNextFrame(IAnimationFrame frame)
        {
            int delay = frame.Delay + Configuration.DelayBetweenFrames;
            if (delay < 0) delay = 0;
            return delay;
        }

		private bool finishedAnimation;
	}
}

