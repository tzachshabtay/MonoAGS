namespace AGS.API
{
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
		/// <code language="lang-csharp">
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
		/// <code language="lang-csharp">
		/// _gameState.RoomTransitions.SetOneTimeNextTransition(AGSRoomTransitions.Dissolve());
		/// </code>
		/// </example>
		void SetOneTimeNextTransition(IRoomTransition transition);
	}
}