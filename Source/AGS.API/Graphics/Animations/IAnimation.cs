using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// Represents an animation. This allows you to configure an animation, and also to query/control animation
    /// when it's running.
    /// </summary>
    public interface IAnimation
	{
        /// <summary>
        /// The list of animation frames that composes the animation (those will run one after the other).
        /// </summary>
        /// <value>The frames.</value>
		IList<IAnimationFrame> Frames { get; }

        /// <summary>
        /// Allows configuring the animation.
        /// </summary>
        /// <value>The configuration.</value>
		IAnimationConfiguration Configuration { get; }

        /// <summary>
        /// Allows querying and controlling the current state of the animation.
        /// </summary>
        /// <value>The state.</value>
		IAnimationState State { get; }

        /// <summary>
        /// Gets the sprite that is currently showing (the current frame's sprite).
        /// </summary>
        /// <value>The sprite.</value>
		ISprite Sprite { get; }

        /// <summary>
        /// Moves the animation to the next frame. This is called by the engine to run the animation.
        /// </summary>
        /// <returns><c>true</c>, if frame was nexted, <c>false</c> otherwise.</returns>
		bool NextFrame();

        /// <summary>
        /// Flips the animation horizontally (from left to right or from right to left). This is applied
        /// to all of the animation frames one by one.
        /// </summary>
		void FlipHorizontally();

        /// <summary>
        /// Flips the animation vertically (from up to down or from down to up). This is applied
        /// to all of the animation frames on by one.
        /// </summary>
		void FlipVertically();

        /// <summary>
        /// Copy this animation and get a new animation. This performs a deep clone.
        /// This means that once you get a clone, the 2 animations are not attached. You can change
        /// a color to one of the sprite in the animation, for example, and this color change will not
        /// be applied on the old animation.
        /// </summary>
        /// <returns>The new animation.</returns>
		IAnimation Clone();
	}
}

