using System.Threading.Tasks;

namespace AGS.API
{    
    /// <summary>
    /// An animation container. This gives access to the animation, and allows to start a new animation
    /// which will replace the old animation.
    /// </summary>
    public interface IAnimationContainer : IComponent
	{
        /// <summary>
        /// The currently associated animation.
        /// </summary>
        /// <value>The animation.</value>
		IAnimation Animation { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the anchor (the pivot point for position/rotate/scale) will 
        /// be drawn on the screen as a cross. This can be used for debugging the game.
        /// </summary>
        /// <value><c>true</c> if debug draw anchor; otherwise, <c>false</c>.</value>
		bool DebugDrawAnchor { get; set; }

        /// <summary>
        /// An event when fires whenever an animation is started on this container.
        /// </summary>
        /// <value>The event.</value>
        IEvent<AGSEventArgs> OnAnimationStarted { get; }

        /// <summary>
        /// Gets or sets a border that will (optionally) surround the animation.
        /// </summary>
        /// <value>The border.</value>
		IBorderStyle Border { get; set; }

        /// <summary>
        /// Starts a new animation (this does not wait for the animation to complete).
        /// </summary>
        /// <param name="animation">Animation.</param>
		void StartAnimation(IAnimation animation);

        /// <summary>
        /// Starts a new animation and waits for it to complete.
        /// Note: if an animation is an endless loop, this method will never return (unless the animation is stopped manually).
        /// </summary>
        /// <returns>A result to indicate if the animation completed successfully or stopped abruptly.</returns>
        /// <param name="animation">Animation.</param>
		AnimationCompletedEventArgs Animate(IAnimation animation);

        /// <summary>
        /// Starts a new animation and returns a task for allowing asynchronously waiting for the animation to complete.
        /// This allows for doing more things in parallel while the animation is playing, while still having the power 
        /// to wait for the animation when you're done.
        /// Note: if an animation is an endless loop, this task will never complete (unless the animation is stopped manually).
        /// </summary>
        /// <example>
        /// Let's animate our 3 musical instruments to play at the same time and wait for the first one to complete.
        /// <code>
        /// var trumpetTask = player1.AnimateAsync(trumpetAnimation);
        /// var pianoTask = player2.AnimateAsync(pianoAnimation);
        /// var drumsTask = player3.AnimateAsync(drumAnimation);
        /// //As we didn't await the tasks yet, the trumpet, piano and drums are playing at the same time.
        /// //Now let's see who finishes first:
        /// var firstTaskToComplete = await Task.WhenAny(trumpetTask, pianoTask, drumsTask);
        /// if (firstTaskToComplete == trumpetTask) await player4.SayAsync("Nice job, trumpet dude!");
        /// else if (firstTaskToComplete == pianoTask) await player4.SayAsync("Nice job, piano dude!");
        /// else await player4.SayAsync("Nice job, drums dude!");
        /// 
        /// //Now let's wait for all animations to complete:
        /// await Task.WhenAll(trumpetTask, pianoTask, drumsTask);
        /// await player4.SayAsync("Nice job, everybody!");
        /// </code>
        /// </example>
        /// <returns>A result to indicate if the animation completed successfully or stopped abruptly.</returns>
        /// <param name="animation">Animation.</param>
		Task<AnimationCompletedEventArgs> AnimateAsync(IAnimation animation);		
	}
}

