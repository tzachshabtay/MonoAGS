namespace AGS.API
{
    public enum SkipCutsceneTrigger
    {
        /// <summary>
        /// Cutscene will be skipped when clicking on escape (the default)
        /// </summary>
        EscapeKeyOnly,
        /// <summary>
        /// Cutscene will be skipped when pressing on any key on the keyboard.
        /// </summary>
        AnyKey,
        /// <summary>
        /// Cutscene will be skipped when pressing on any key on the keyboard or any mouse button click.
        /// </summary>
        AnyKeyOrMouse,
        /// <summary>
        /// Cutscene will not be skipped automatically, you have to program it yourself by calling the Skip command.
        /// </summary>
        Custom,
    }

    /// <summary>
    /// A cutscene is used to wrap a portion of the game together. This portion can then be skipped by the user.
    /// This is useful for things like introduction sequences, where you want the player to be able to skip over an intro that they've seen before. 
    /// </summary>
    public interface ICutscene
    {
        /// <summary>
        /// Gets or sets what triggers skipping a cutscene (by default, pressing escape only to skip a scene,
        /// but this could be changed to be any key on the keyboard, any key or mouse click, or a custom skip trigger that you program yourself).
        /// </summary>
        /// <value>The skip trigger.</value>
        SkipCutsceneTrigger SkipTrigger { get; set; }

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
        /// 
        /// You need to mark the end of the cutscene with the End command.
        /// Be very careful with where you place the corresponding End command. The script must pass through it in its normal run in order for the skipping to work - otherwise, when the player presses ESC the game could appear to hang.
        /// </summary>
		void Start();

        /// <summary>
        /// Marks the end of a cutscene. If the player skips the cutscene, the game will fast-forward to this point.
        /// Be very careful with where you place the corresponding End command. The script must pass through it in its normal run in order for the skipping to work - otherwise, when the player presses ESC the game could appear to hang.
        /// This function returns false if the player watched the cutscene, or true if they skipped it.
        /// </summary>
		bool End();

        /// <summary>
        /// Programmatically instructs the cutscene to start skipping (if a cutscene is running).
        /// </summary>
        void BeginSkip();

        /// <summary>
        /// Copies data from a different cutscene object.
        /// </summary>
        /// <param name="cutscene">The cutscene object to copy data from.</param>
		void CopyFrom(ICutscene cutscene);
    }
}