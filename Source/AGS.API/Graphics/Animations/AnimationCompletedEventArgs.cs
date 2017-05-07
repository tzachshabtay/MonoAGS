using System;

namespace AGS.API
{
    /// <summary>
    /// Event arguments for when an animation completes.
    /// </summary>
	public class AnimationCompletedEventArgs : EventArgs
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.AnimationCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="completedSuccessfully">If set to <c>true</c> completed successfully.</param>
		public AnimationCompletedEventArgs (bool completedSuccessfully)
		{
			CompletedSuccessfully = completedSuccessfully;
		}

        /// <summary>
        /// Gets a value indicating whether this animation completed successfully.
        /// </summary>
        /// <value><c>true</c> if completed successfully; otherwise, <c>false</c>.</value>
		public bool CompletedSuccessfully { get; private set; }
	}
}

