namespace AGS.API
{
	/// <summary>
	/// The room transition state.
	/// </summary>
	public enum RoomTransitionState
	{
		/// <summary>
		/// We're not currently in a room transition (we're in a normal rendering loop).
		/// </summary>
		NotInTransition,
		/// <summary>
		/// A room transition has started, we haven't left the old room yet.
		/// </summary>
		BeforeLeavingRoom,
        /// <summary>
		/// We're preparing the transition screenshots (we need to take a screenshot of the old room,
		/// then move the player to the new room, set up the camera and take another screenshot).
		/// </summary>
		PreparingTransition,
        /// <summary>
        /// We left the old room, we need to prepare the display list for the new room before taking the screenshot to
        /// be used by the transition.
        /// </summary>
        PreparingNewRoomDisplayList,
		/// <summary>
		/// We're currently in transition. We have the 2 screenshots, which can be manipulated by the room transition.
		/// </summary>
		InTransition,
		/// <summary>
		/// We're almost finished with the room transition. We entered the new room, giving the transition
		/// one last opportunity to do some magic.
		/// </summary>
		AfterEnteringRoom,
	}

	/// <summary>
	/// Allows to set the room transitions (and to get some information on the transitions)
	/// </summary>
	public interface IRoomTransitions
	{
		/// <summary>
		/// Gets or sets the room transition.
		/// All future room transitions will use this transition until a new one has been set.
		/// </summary>
		/// <value>The transition.</value>
		/// <example>
		/// Let's set a box out transition:
		/// <code>
		/// _gameState.RoomTransitions.Transition = AGSRoomTransitions.BoxOut();
		/// </code>
		/// </example>
		IRoomTransition Transition { get; set; }

		/// <summary>
		/// Sets a one time transition. This transition will only be used once (for the next room transition),
		/// and then the old transition will be used again.
		/// </summary>
		/// <param name="transition">The room transition to be used once.</param>
		/// <example>
		/// Let's set a dissolve transition, only for the next room transition:
		/// <code>
		/// _gameState.RoomTransitions.SetOneTimeNextTransition(AGSRoomTransitions.Dissolve());
		/// </code>
		/// </example>
		void SetOneTimeNextTransition(IRoomTransition transition);

		/// <summary>
		/// Gets the current state of the room transition.
		/// </summary>
		/// <value>The state.</value>
		RoomTransitionState State { get; }
	}
}

