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
        /// The animation that is used whenever the character is walking somewhere.
        /// </summary>
        /// <example>
        /// <code>
        /// cPlayer.Outfit.WalkAnimation = aCrazyWalkAnimation;
        /// cPlayer.Walk(100, 200); //The crazy walk animation will now be used for walking to (100,200).
        /// </code>
        /// </example>
		IDirectionalAnimation WalkAnimation { get; set; }

        /// <summary>
        /// The animation that is used whenever the character is standing.
        /// </summary>
        /// <example>
        /// <code>
        /// cPlayer.Outfit.IdleAnimation = aPlayingYoyoAnimation; //The character will play yoyo when idle        
        /// </code>
        /// </example>
		IDirectionalAnimation IdleAnimation { get; set; }

        /// <summary>
        /// The animation that is used whenever the character is speaking.
        /// </summary>
        /// <example>
        /// <code>
        /// cPlayer.Outfit.SpeakAnimation = aScaryAnimation;
        /// cPlayer.Say("Do I scare you?"); //The character will speak with the scary animation
        /// </code>
        /// </example>
		IDirectionalAnimation SpeakAnimation { get; set; }
    }
}