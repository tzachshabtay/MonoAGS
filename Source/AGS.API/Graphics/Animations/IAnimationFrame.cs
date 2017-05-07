namespace AGS.API
{
    /// <summary>
    /// An animation frame (an animation is composed from a list of frames, which change all the time
    /// to make the object looks like it's animated).
    /// </summary>
    public interface IAnimationFrame
	{
        /// <summary>
        /// Gets or sets the sprite that is shown for this frame.
        /// </summary>
        /// <value>The sprite.</value>
		ISprite Sprite { get; set; }

        /// <summary>
        /// Gets or sets the sound emitter from this frame. If a sound emitter is attached to this frame
        /// and also has an attached audio clip, the audio clip will play when the frame is showing.
        /// You don't have to call this yourself, the sound emitter itself has convenience methods for assigning
        /// itself to several animations at once.
        /// </summary>
        /// <value>The sound emitter.</value>
		ISoundEmitter SoundEmitter { get; set; }

        /// <summary>
        /// An animation frame has 2 modes of delays (how much time to wait before moving on to the next frame).
        /// It's either a specific delay or a random delay with an allowed range.
        /// A random delay with an allowed range is useful for simulating talking, for example, when the mouth 
        /// is moving in random speeds, giving the illusion of actual talking, instead of repeating animation 
        /// that breaks the illusion fast.
        /// Anyways, the delay is added on top of the animation's overall delay (see cref="IAnimationConfiguration"/>)
        /// and measure in game ticks. So let's say we have an overall animation speed configured to be 5 (the default),
        /// and we have this frame delay configured to be also 5 (the default is 0) and the game is running with 60 FPS
        /// (frames per second- also the default), then the total delay for this frame will be 10 ticks, so it will run
        /// for 1/6 of a second.
        /// 
        /// When setting the delay here, this means you want a deterministic delay (not randomly generated each time),
        /// and then you can also query the delay here.
        /// If you set the MinDelay and MaxDelay of a frame it means you want a random delay for the frame, so querying
        /// the delay here will give you a random value which will not be the actual current delay of the frame.
        /// </summary>
        /// <value>The delay.</value>
		int Delay { get; set; }

        /// <summary>
        /// Gets or sets the minimum delay for generating random frame delays.
        /// Both MinDelay and MaxDelay should be set for a random frame delay.
        /// If MinDelay is 5 and MaxDelay is 10, then each time this frame runs it will delay an extra 5-10 ticks
        /// before switching to the next frame.
        /// <seealso cref="Delay"/>
        /// <seealso cref="MaxDelay"/>
        /// </summary>
        /// <value>The minimum delay.</value>
		int MinDelay { get; set; }

        /// <summary>
        /// Gets or sets the maximum delay for generating random frame delays.
        /// Both MinDelay and MaxDelay should be set for a random frame delay.
        /// If MinDelay is 5 and MaxDelay is 10, then each time this frame runs it will delay an extra 5-10 ticks
        /// before switching to the next frame.
        /// <seealso cref="Delay"/>
        /// <seealso cref="MinDelay"/>
        /// </summary>
        /// <value>The maximum delay.</value>
		int MaxDelay { get; set; }

        /// <summary>
        /// Clones the animation frame (performs a deep clone, so the new frame will be completely detached from the old frame).
        /// </summary>
        /// <returns>The new cloned frame.</returns>
		IAnimationFrame Clone();
	}
}

