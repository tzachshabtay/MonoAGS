namespace AGS.API
{
    public enum LoopingStyle
	{
		/// <summary>
		/// Will animate the loop forwards, and then start again from the top
		/// </summary>
		Forwards,
		/// <summary>
		/// Will animate the loop backwards, and then start again from the bottom
		/// </summary>
		Backwards,
		/// <summary>
		/// Will animate the loop forwards, and then it will go back, and start again
		/// </summary>
		ForwardsBackwards,
		/// <summary>
		/// Will animate the loop backwards, and then will it will go forwards, and start again
		/// </summary>
		BackwardsForwards,
	}

	public interface IAnimationConfiguration
	{
        /// <summary>
        /// Allows to configure how the animation cycles (does it run forwards, backwards, does it zig zag?)
        /// </summary>
        /// <value>The looping.</value>
		LoopingStyle Looping { get; set; }

		/// <summary>
		/// Allows to configure the number of loops the animation will run before completing. 
        /// 0 for an endless animation.
		/// </summary>
		/// <value>The number of loops.</value>
		int Loops { get; set; }

        /// <summary>
        /// Gets or sets the delay between each frame.
        /// The delay is measured in frames, so if we're running at the expected 60 FPS, a delay of 5 means
        /// each second 12 frames of animation will be shown.
        /// Note that each frame can be configured with an additional delay. That delay will be added for this
        /// overall delay for that specific frame, so a frame's delay is relative, while this delay can be used
        /// as an overall speed for the entire animation.
        /// </summary>
        /// <value>The delay between frames.</value>
        int DelayBetweenFrames { get; set; }
	}
}

