using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// An outfit is a collection of animations that are associated with a character.
    /// The collection can be swapped with another collection when the character changes his look.
    /// </summary>
    /// <example>
    /// Say your player can wear a jacket. You can create two outfits in advance, one with a jacket
    /// and the other without. Each outfit will have different walk, idle and speak animations (one with a jacket and 
    /// the other without).
    /// </example>
    /// <code>
    /// private void onJacketClicked()
    /// {
    ///     cPlayer.Say("Let's wear the jacket!"); //The character speaks with the "no jacket" outfit
    ///     cPlayer.Outfit = oJacket;
    ///     cPlayer.Say("How does it look?"); //The character now speaks with the jacket outfit
    /// }
    /// </code>    
    public interface IOutfit
    {
        /// <summary>
        /// Gets or sets an animation that will be associated with a specific key for this outfit.
        /// Some built in keys are expected by some components to change animations (idle, walk, speak).
        /// Custom animations can be added and accessed from the outfit as needed.
        /// <example>
        /// <code>
        /// cPlayer.Outfit[AGSOutfit.Walk] = aCrazyWalkAnimation;
        /// cPlayer.Outfit[AGSOutfit.Idle] = aPlayingYoyoAnimation;
        /// cPlayer.Outfit[AGSOutfit.Speak] = aScaryAnimation;
        /// cPlayer.Outfit["Jump"] = aJumpAnimation;
        /// 
        /// cPlayer.Walk(100, 200); //The crazy walk animation will now be used for walking to (100,200).
        /// Thread.Sleep(5000); //The player has finished walking, and sitting idle for 5 seconds during which the playing yoyo animation is playing
        /// cPlayer.Say("Do I scare you?"); //The character will speak with the scary animation
        /// cPlayer.StartAnimation(cPlayer.Outfit["Jump"].Left); //The character will do a jump left animation.
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="key">Key.</param>
        IDirectionalAnimation this[string key] { get; set; }

        /// <summary>
        /// Gets all the animations as a dictionary;
        /// </summary>
        /// <returns>The dictionary.</returns>
        IDictionary<string, IDirectionalAnimation> ToDictionary();
    }
}