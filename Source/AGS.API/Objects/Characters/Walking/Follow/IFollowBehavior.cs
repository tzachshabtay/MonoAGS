namespace AGS.API
{
	[RequiredComponent(typeof(IWalkBehavior))]
	[RequiredComponent(typeof(IHasRoom))]
	[RequiredComponent(typeof(ITranslateComponent))]
    /// <summary>
    /// This component adds the ability for a character to follow another entity (i.e to keep walking
    /// to where the entity is. The follow settings can be configured to set how aggressive that follow is). 
    /// </summary>
	public interface IFollowBehavior : IComponent
	{
        /// <summary>
        /// Starts following the specified object throughout the game. The character will walk to where
        /// the object is at, where the follow settings can be configured to say how aggressively to follow,
        /// and to what distance from the target.
        /// Calling Follow with another object will stop following the current object and start following the new object.
        /// To stop following altogether, call the Follow again with null as the target object.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="settings">Settings.</param>
		void Follow(IObject obj, IFollowSettings settings = null); 

        /// <summary>
        /// Gets the target that is currently being followed (or null if not currently following anybody).
        /// </summary>
        /// <value>The target being followed.</value>
        IObject TargetBeingFollowed { get; }
	}
}

