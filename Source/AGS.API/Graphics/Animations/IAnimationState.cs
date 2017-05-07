using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// Control and query the animation current state.
    /// </summary>
    public interface IAnimationState
	{
        /// <summary>
        /// Gets or sets a value indicating whether this animation is currently running backwards.
        /// </summary>
        /// <value><c>true</c> if running backwards; otherwise, <c>false</c>.</value>
		bool RunningBackwards { get; set; }

        /// <summary>
        /// Gets or sets the current animation frame.
        /// </summary>
        /// <value>The current frame.</value>
		int CurrentFrame { get; set; }

        /// <summary>
        /// Gets or sets the current loop.
        /// </summary>
        /// <value>The current loop.</value>
		int CurrentLoop { get; set; }

        /// <summary>
        /// Gets or sets the time to next frame (in game ticks: so 10 when the game is at 60FPS means 1/6 of a second).
        /// </summary>
        /// <value>The time to next frame.</value>
		int TimeToNextFrame { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this animation is paused.
        /// </summary>
        /// <value><c>true</c> if is paused; otherwise, <c>false</c>.</value>
		bool IsPaused { get; set; }

        /// <summary>
        /// Allows to subscribe to a task and await animation completion (note that if the animation
        /// is configured to loop forever, the task will never complete).
        /// </summary>
        /// <example>
        /// <code>
        /// await cHero.SayAsync("Waiting for the animation to complete");
        /// await state.OnAnimationCompleted.Task;
        /// await cHero.SayAsync("Done!");
        /// </code>
        /// </example>
        /// <value>The on animation completed.</value>
		TaskCompletionSource<AnimationCompletedEventArgs> OnAnimationCompleted { get; }
	}
}

