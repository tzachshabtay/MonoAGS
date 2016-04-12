namespace AGS.API
{
    /// <summary>
    /// A cutscene is used to wrap a portion of the game together. This portion can then be skipped by the user.
    /// This is useful for things like introduction sequences, where you want the player to be able to skip over an intro that they've seen before. 
    /// </summary>
    public interface ICutscene
    {
        /// <summary>
        /// Returns whether the player has elected to skip the current cutscene. This will return true if the game is between a Start and End command, and the player has chosen to skip it.
        /// Although cutscene skipping is handled automatically by AGS, you can use this property to optimise the process by bypassing any lengthy blocks of code that don't need to 
        /// be run if the cutscene is being skipped over.        
        /// </summary>
        /// <example>
        /// <code>
        /// if (!cutscene.IsSkipping)
        /// {
        ///    aScaryMusic.Play();
        ///    Thread.Sleep(100);
        ///    aScaryMusic.Stop();
        /// }
        /// </code>
        /// </example>
        bool IsSkipping { get; }

        /// <summary>
        /// When the player chooses to skip a cutscene all of the script code is run as usual, but any blocking commands are run through without the usual game cycle delays. 
        /// Therefore, you should never normally need to use this property since cutscenes should all be handled automatically, but it could be useful for script modules.
        /// <example>
        /// <code>
        /// if (cutscene.IsRunning)
        /// {
        ///    //Do Something
        /// }
        /// </code>
        /// </example>
        /// </summary>
		bool IsRunning { get; }

        /// <summary>
        /// Marks the start of a cutscene. Once your script passes this point, the player can choose to skip a portion by pressing a pre-configured button. 
        /// This is useful for things like introduction sequences, where you want the player to be able to skip over an intro that they've seen before.
        /// </summary>
		void Start();

        /// <summary>
        /// Marks the end of a cutscene. If the player skips the cutscene, the game will fast-forward to this point.
        /// </summary>
		void End();

        /// <summary>
        /// Copies data from a different cutscene object.
        /// </summary>
        /// <param name="cutscene">The cutscene object to copy data from.</param>
		void CopyFrom(ICutscene cutscene);
    }
}