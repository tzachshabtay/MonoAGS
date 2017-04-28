using System;

namespace AGS.API
{
    /// <summary>
    /// Allows to set how cross fading audio clips between rooms behaves.
    /// </summary>
	public interface ICrossFading
	{
        /// <summary>
        /// Should the audio clip for the old room fade out while transistioning to the new room?
        /// </summary>
        /// <value><c>true</c> if fade out; otherwise, <c>false</c>.</value>
		bool FadeOut { get; set; }

        /// <summary>
        /// Should the audio clip for the new room fade in while transistioning to the new room?
        /// </summary>
        /// <value><c>true</c> if fade in; otherwise, <c>false</c>.</value>
		bool FadeIn { get; set; }

        /// <summary>
        /// If the audio clip for the old room is set to fade out, how long (in seconds) should the fade out take?
        /// </summary>
        /// <value>The fade out seconds.</value>
		float FadeOutSeconds { get; set; }

        /// <summary>
        /// If the audio clip for the new room is set to fade in, how long (in seconds) should the fade in take?
        /// </summary>
        /// <value>The fade in seconds.</value>
		float FadeInSeconds { get; set; }

        /// <summary>
        /// If the audio clip for the old room is set to fade out, this allows you to set the easing function for the fade out (linear, ease-in, etc).
        /// </summary>
        /// <example>
        /// <code>
        /// game.AudioSettings.RoomMusicCrossFading.EaseFadeOut = Ease.QuadOut;
        /// </code>
        /// </example>
        /// <value>The ease fade out.</value>
		Func<float, float> EaseFadeOut { get; set; }

        /// <summary>
        /// If the audio clip for the new room is set to fade in, this allows you to set the easing function for the fade in (linear, ease-in, etc).
        /// </summary>
        /// <example>
        /// <code>
        /// game.AudioSettings.RoomMusicCrossFading.EaseFadeIn = Ease.CubeInOut;
        /// </code>
        /// </example>
        /// <value>The ease fade out.</value>
		Func<float, float> EaseFadeIn { get; set; }
	}
}

