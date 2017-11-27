using AGS.API;

namespace AGS.Engine
{
    /// <summary>
    /// Configuration and state required for repeating a tween.
    /// </summary>
    public class TweenRepeatInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether this animation is currently running backwards.
        /// </summary>
        /// <value><c>true</c> if running backwards; otherwise, <c>false</c>.</value>
        public bool RunningBackwards { get; set; }

        /// <summary>
        /// Gets or sets the current loop.
        /// </summary>
        /// <value>The current loop.</value>
        public int CurrentLoop { get; set; }

        /// <summary>
        /// Allows to configure the number of loops the animation will run before completing. 
        /// 0 for an endless animation.
        /// </summary>
        /// <value>The number of loops.</value>
        public int TotalLoops { get; set; }

        /// <summary>
        /// Allows to configure how the animation cycles (does it run forwards, backwards, does it zig zag?)
        /// </summary>
        /// <value>The looping.</value>
        public LoopingStyle Looping { get; set; }

        /// <summary>
        /// Gets or sets the delay between loops in seconds.
        /// </summary>
        /// <value>The delay between loops in seconds.</value>
        public float DelayBetweenLoopsInSeconds { get; set; }

        /// <summary>
        /// Called by the tweening engine to advance the repeating tween to the next loop.
        /// </summary>
        /// <returns><c>true</c>, if loop was advanced, <c>false</c> if tween should complete.</returns>
        public bool NextLoop()
        {
            if (CurrentLoop >= TotalLoops && TotalLoops > 0) return false;
            CurrentLoop++;
            if (Looping == LoopingStyle.BackwardsForwards || Looping == LoopingStyle.ForwardsBackwards)
            {
                RunningBackwards = !RunningBackwards;
            }
            return true;
        }
    }
}
