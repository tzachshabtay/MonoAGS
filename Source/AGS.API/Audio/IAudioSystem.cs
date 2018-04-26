using System.Collections.ObjectModel;

namespace AGS.API
{
    /// <summary>
    /// The entry point for interacting with the audio system.
    /// </summary>
    public interface IAudioSystem
    {
        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        IAudioSettings Settings { get; }

        /// <summary>
        /// Gets the currently playing sounds.
        /// </summary>
        /// <value>The currently playing sounds.</value>
        ReadOnlyCollection<ISound> CurrentlyPlayingSounds { get; }

        /// <summary>
        /// Gets all available audio clips.
        /// </summary>
        /// <value>The audio clips.</value>
        IAGSBindingList<IAudioClip> AudioClips { get; }

        /// <summary>
        /// Gets the audio rules (which can modify properties of playing sounds).
        /// </summary>
        /// <value>The audio rules.</value>
        IAGSBindingList<IAudioRule> AudioRules { get; }
    }
}