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
	    /// An optional minimum x offset for when wandering off (behaves similarly to <see cref="MinXOffset"/>, just for the 'wander off' phase).
	    /// If left out, the minimum x will be the left-most part of the room (regardless for where the target is).
	    /// </summary>
	    /// <value>The minimum X Offset for wandering off.</value>
	    float? MinXOffsetForWanderOff { get; }

	    /// <summary>
	    /// An optional maximum x offset for when wandering off (behaves similarly to <see cref="MaxXOffset"/>, just for the 'wander off' phase).
	    /// If left out, the maximum x will be the right-most part of the room (regardless for where the target is).
	    /// </summary>
	    /// <value>The maximum X Offset for wandering off.</value>
	    float? MaxXOffsetForWanderOff { get; }

	    /// <summary>
	    /// An optional minimum y offset for when wandering off (behaves similarly to <see cref="MinYOffset"/>, just for the 'wander off' phase).
	    /// If left out, the minimum y will be the bottom-most part of the room (regardless for where the target is).
	    /// </summary>
	    /// <value>The minimum y Offset for wandering off.</value>
	    float? MinYOffsetForWanderOff { get; }

	    /// <summary>
	    /// An optional maximum y offset for when wandering off (behaves similarly to <see cref="MaxYOffset"/>, just for the 'wander off' phase).
	    /// If left out, the maximum y will be the top-most part of the room (regardless for where the target is).
	    /// </summary>
	    /// <value>The maximum Y Offset for wandering off.</value>
	    float? MaxYOffsetForWanderOff { get; }

        /// <summary>
        /// The probability (in percentage) that the next character walk will not actually be towards the target,
        /// but wandering off to somewhere else in the room.
        /// </summary>
        /// <value>The wander off percentage.</value>
		int WanderOffPercentage { get; }
	    
	    /// <summary>
	    /// The probability (in percentage) that the character will not move in the next walk, if she/he is already
	    /// within allowed range of the target.
	    /// </summary>
	    int StayPutPercentage { get; }
	    
	    /// <summary>
	    /// The probability (in percentage) that the character will stay on the same side of the target on the X axis (as opposed to getting around it)
	    /// </summary>
	    int StayOnTheSameSideForXPercentage { get; }

	    /// <summary>
	    /// The probability (in percentage) that the character will stay on the same side of the target on the Y axis (as opposed to getting around it)
	    /// </summary>
	    int StayOnTheSameSideForYPercentage { get; }
	    
	    /// <summary>
	    /// It might look silly for a character to walk just a few pixels. This allows you to set the minimum distance
	    /// from its current position that the follower will walk. 
	    /// </summary>
	    float MinimumWalkingDistance { get; }

        /// <summary>
        /// Should the character follow the target when the target moves to a different room?
        /// </summary>
        /// <value><c>true</c> if follow between rooms; otherwise, <c>false</c>.</value>
		bool FollowBetweenRooms { get; }
	}
}

