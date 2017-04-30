namespace AGS.API
{
    /// <summary>
    /// Configures how the character follow component behaves.
    /// </summary>
	public interface IFollowSettings
	{
        /// <summary>
        /// Gets the minimum wait (in game ticks) the character wait between walks.
        /// This requires setting <see cref="MaxWaitBetweenWalks"/> as well, and the wait time will be randomized
        /// each time between the minimum and maximum wait times.
        /// </summary>
        /// <value>The minimum wait between walks.</value>
		int MinWaitBetweenWalks { get; }

        /// <summary>
        /// Gets the maximum wait (in game ticks) the character wait between walks.
        /// This requires setting <see cref="MinWaitBetweenWalks"/> as well, and the wait time will be randomized
        /// each time between the minimum and maximum wait times.
        /// </summary>
        /// <value>The minimum wait between walks.</value>
		int MaxWaitBetweenWalks { get; }

        /// <summary>
        /// The minimum horizontal distance the follower will keep from the target.
        /// This requires setting <see cref="MaxXOffset"/> as well, and the distance will be randomized
        /// each time between the minimum and maximum x.
        /// </summary>
        /// <value>The minimum X Offset.</value>
		float MinXOffset { get; }

        /// <summary>
        /// The maximum horizontal distance the follower will keep from the target.
        /// This requires setting <see cref="MinXOffset"/> as well, and the distance will be randomized
        /// each time between the minimum and maximum x.
        /// </summary>
        /// <value>The maximum X Offset.</value>
		float MaxXOffset { get; }

        /// <summary>
        /// The minimum vertical distance the follower will keep from the target.
        /// This requires setting <see cref="MaxYOffset"/> as well, and the distance will be randomized
        /// each time between the minimum and maximum y.
        /// </summary>
        /// <value>The minimum Y Offset.</value>
		float MinYOffset { get; }

        /// <summary>
        /// The minimum vertical distance the follower will keep from the target.
        /// This requires setting <see cref="MinYOffset"/> as well, and the distance will be randomized
        /// each time between the minimum and maximum y.
        /// </summary>
        /// <value>The maximum Y Offset.</value>
		float MaxYOffset { get; }

        /// <summary>
        /// The probability (in percentage) that the next character walk will not actually be towards the target,
        /// but wandering off to somewhere else in the room.
        /// </summary>
        /// <value>The wander off percentage.</value>
		int WanderOffPercentage { get; }

        /// <summary>
        /// Should the character follow the target when the target moves to a different room?
        /// </summary>
        /// <value><c>true</c> if follow between rooms; otherwise, <c>false</c>.</value>
		bool FollowBetweenRooms { get; }
	}
}

