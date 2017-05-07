using System;

namespace AGS.API
{
    /// <summary>
    /// Event arguments which allows you to modify how/where the text is rendered and control
    /// how the speech is skipped (assuming you configure <see cref="SkipText.External"/>  in your <see cref="ISayConfig"/>).
    /// </summary>
	public class BeforeSayEventArgs : AGSEventArgs
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.BeforeSayEventArgs"/> class.
        /// </summary>
        /// <param name="label">Label.</param>
        /// <param name="skip">Skip.</param>
		public BeforeSayEventArgs(ILabel label, Action skip)
		{
			Label = label;
			Skip = skip;
		}

        /// <summary>
        /// The text label which will be displayed to the user.
        /// </summary>
        /// <value>The label.</value>
		public ILabel Label { get; set; }

        /// <summary>
        /// An action you can call to skip the text, for implementing custom text skipping.
        /// </summary>
        /// <value>The skip.</value>
		public Action Skip { get; private set; }
	}
}

