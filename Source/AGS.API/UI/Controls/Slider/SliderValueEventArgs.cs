namespace AGS.API
{
    /// <summary>
    /// Event arguments for slider value change.
    /// </summary>
    public class SliderValueEventArgs : AGSEventArgs
	{
		public SliderValueEventArgs(float value)
		{
			Value = value;
		}

        /// <summary>
        /// Gets the slider's value.
        /// </summary>
        /// <value>The value.</value>
		public float Value { get; private set; }
	}
}

