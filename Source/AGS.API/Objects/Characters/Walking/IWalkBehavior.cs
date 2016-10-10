using System.Threading.Tasks;

namespace AGS.API
{
    [RequiredComponent(typeof(IAnimationContainer), false)] //Only needed if "MovementLinkedToAnimation" is enabled
	public interface IWalkBehavior : IComponent
	{
        /// <summary>
        /// Gets or sets the walk step.
        /// The walk speed is actually the number of pixels the character moves each time he/she/it moves.
        /// If <see cref="MovementLinkedToAnimation"/> is turned off, the step would be made on each frame, making for a smooth movement.
        /// This means that for this state, the walk step is the sole 'decision maker' for the walking speed of the character.
        /// If <see cref="MovementLinkedToAnimation"/> is turned on (the default), the step would be made each time the animation frame changes.
        /// This means that for this state, the walk step should be setup once (it needs to match exactly the amount of pixels the leg moves in the drawings)
        /// when the actual controller of the walk speed is the animation's speed (the animation's <see cref="T:AGS.API.IAnimationConfiguration.DelayBetweenFrames"/> property 
        /// and an optional additional delay for the actual running frame).
        /// </summary>
        /// <value>The walk speed.</value>
		PointF WalkStep { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.IWalkBehavior"/> adjust the walk speed to
        /// the scaling area (i.e for a scaling area that shrinks the character to simulate a far away place, you'll want the character to walk slower).
        /// </summary>
        /// <value><c>true</c> if adjust walk speed to scale area; otherwise, <c>false</c>.</value>
        bool AdjustWalkSpeedToScaleArea { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.IWalkBehavior"/> movement is linked to the animation.
        /// This is on by default, meaning that the walk step would only be performed when the walk animation frame changes.
        /// For the walk to look right, the images need to be drawn so that a foot touching the ground moves back a 
        /// constant amount of pixels in between frames. This amount should then be entered as the <see cref="WalkStep"/>, 
        /// and not touched any further. Any speed adjustment should only done by changing the animation speed (the animation's <see cref="T:AGS.API.IAnimationConfiguration.DelayBetweenFrames"/> property 
        /// and an optional additional delay for the actual running frame).
        /// 
        /// If this is turned off, then the walk step would be performed on each frame making for a smooth movement.
        /// Note that if the walk animation only has a single frame, then the engine will ignore the configuration
        /// and will treat it as false.
        /// 
        /// How to decide if this should be on or off?
        /// ------------------------------------------
        /// As a rule of thumb, if the character has legs you would want this on.
        /// If you turn this off (for a character which has legs) while the movement will look smoother, it
        /// will also cause a gliding effect, caused by the fact that the movement is not in sync with the animation
        /// of the moving feet.
        /// If your character doesn't have legs (a robot, a floating ghost, etc), turning this off would make
        /// for a smoother looking walk.
        /// </summary>
        /// <value><c>true</c> if movement linked to animation; otherwise, <c>false</c>.</value>
        bool MovementLinkedToAnimation { get; set; }

        /// <summary>
        /// Gets a value indicating whether the character is walking.
        /// </summary>
        /// <value><c>true</c> if is walking; otherwise, <c>false</c>.</value>
		bool IsWalking { get; }

        /// <summary>
        /// Gets the destination the character is currently walking to (if currently walking),
        /// or the last destination the character was walking to (if currently not walking).
        /// </summary>
        /// <value>The walk destination.</value>
        ILocation WalkDestination { get; }

        /// <summary>
        /// If set to true, lines will be drawn to mark the selected paths to walk by the path finder.
        /// This can be useful for debugging issues with the path finder algorithm.
        /// </summary>
        /// <value><c>true</c> if debug draw walk path; otherwise, <c>false</c>.</value>
		bool DebugDrawWalkPath { get; set; }

        /// <summary>
        /// Walk to the specified location.
        /// Returns true if the walk was completed successfully, or false if the walk was cancelled (if the user clicked on something else).
        /// <example>
        /// <code>
        /// cHero.Walk(oChair.Location); //will walk to the chair
        /// cHero.Walk(new AGSLocation(oChair.X - 15f, oChair.Y)); //will walk to the left of the chair
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="location">Location.</param>
		bool Walk(ILocation location);

        /// <summary>
        /// Walks asynchronously to the specified location.
        /// <example>
        /// Let's start walking to the chair, say something about it while we're walking,
        /// and then properly respond if the user decided they want to walk somewhere else before we got to the chair.
        /// <code>
        /// Task<bool> walkSuccessful = cHero.WalkAsync(oChair.Location);
        /// await cHero.SayAsync("I'm walking to the chair!");
        /// if (await walkSuccessful)
        /// {
        ///     cHero.Say("And now I'm sitting!");
        ///     sitOnChair();
        /// }
        /// else
        /// {
        ///     cHero.Say("You know what, that the chair doesn't look too comfortable, I think I'll pass.");
        /// }
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>Returns true if the walk was completed successfully, or false if the walk was cancelled (if the user clicked on something else).</returns>
        /// <param name="location">The location.</param>
		Task<bool> WalkAsync(ILocation location);

        /// <summary>
        /// Stops the current walk (if there is a walk to stop).
        /// </summary>
		void StopWalking();

        /// <summary>
        /// Asynchronosly stops the current walk (if there is a walk to stop).
        /// </summary>
        /// <returns>The walking async.</returns>
		Task StopWalkingAsync();

        /// <summary>
        /// If the character is already standing in a walkable area, does nothing.
        /// Otherwise it would try to place it on a walkable area close by.
        /// </summary>
		void PlaceOnWalkableArea();
	}
}

