namespace AGS.API
{
    /// <summary>
    /// A component that acts as a control to edit numbers.
    /// </summary>
    public interface INumberEditorComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the edited number.
        /// </summary>
        /// <value>The value.</value>
        float Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.INumberEditorComponent"/> edits whole
        /// numbers only (ints), or numbers with decimal digits (floats).
        /// </summary>
        /// <value><c>true</c> if edit whole numbers only; otherwise, <c>false</c>.</value>
        bool EditWholeNumbersOnly { get; set; }

        /// <summary>
        /// Gets or sets the step that increases/decreases the value when pressing on the up/down arrow keys/buttons.
        /// </summary>
        /// <value>The step.</value>
        float Step { get; set; }

        /// <summary>
        /// Gets or sets the minimum allowed value.
        /// </summary>
        /// <value>The minimum value.</value>
        float? MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed value.
        /// </summary>
        /// <value>The max value.</value>
        float? MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the suggested minimum value.
        /// This will be the minimum value shown in the slider (if one exists), but will not be forced on a user which types a value by hand.
        /// </summary>
        /// <value>The suggested minimum value.</value>
        float? SuggestedMinValue { get; set; }

        /// <summary>
        /// Gets or sets the suggested maximum value.
        /// This will be the maximum value shown in the slider (if one exists), but will not be forced on a user which types a value by hand.
        /// </summary>
        /// <value>The suggested maximum value.</value>
        float? SuggestedMaxValue { get; set; }

        /// <summary>
        /// Gets or sets an optional button that will increase the value by <see cref="Step"/> when clicked.
        /// </summary>
        /// <value>Up button.</value>
        IButton UpButton { get; set; }

        /// <summary>
        /// Gets or sets an optional button that will decrease the value by <see cref="Step"/> when clicked.
        /// </summary>
        /// <value>Up button.</value>
        IButton DownButton { get; set; }

        /// <summary>
        /// Gets or sets an optional slider which will allow editing the value by sliding the handle.
        /// The range of the slider is determined by either <see cref="SuggestedMinValue"/> and <see cref="SuggestedMaxValue"/>
        /// if those exist, otherwise by <see cref="MinValue"/> and <see cref="MaxValue"/> if those exist, otherwise the
        /// arbitrary -1000 and 1000 are set as minimum and maximum.
        /// </summary>
        /// <value>The slider.</value>
        ISlider Slider { get; set; }

        /// <summary>
        /// An event which fires whenever the value is changed.
        /// </summary>
        /// <value>The event.</value>
        IBlockingEvent OnValueChanged { get; }
    }
}
