using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// This component adds the ability for a character to follow another entity (i.e to keep walking
    /// to where the entity is. The follow settings can be configured to set how aggressive that follow is). 
    /// </summary>
    [RequiredComponent(typeof(IWalkComponent))]
	[RequiredComponent(typeof(IHasRoomComponent))]
	[RequiredComponent(typeof(ITranslateComponent))]
    public interface IFollowComponent : IComponent
	{
        /// <summary>
        /// Starts following the specified object throughout the game. The character will walk to where
        /// the object is at, where the follow settings can be configured to say how aggressively to follow,
        /// and to what distance from the target.
        /// Calling Follow with another object will stop following the current object and start following the new object.
        /// To stop following altogether, call the Follow again with null as the target object. Note however, that if you need to have the character
        /// walk after stopping following, you should use <see cref="StopFollowingAsync"/>.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="settings">Settings.</param>
		void Follow(IObject obj, IFollowSettings settings = null); 

        /// <summary>
        /// Gets the target that is currently being followed (or null if not currently following anybody).
        /// </summary>
        /// <value>The target being followed.</value>
        IObject TargetBeingFollowed { get; }

        /// <summary>
        /// Stops following the current target.
        /// The task that is returned should be waited on, if you want to have the character walk somewhere after this
        /// (otherwise you might get a StopWalking command issues from the StopFollowing command which will stop your walk).
        /// </summary>
        /// <returns>The following async.</returns>
        Task StopFollowingAsync();
    }
}