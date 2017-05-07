namespace AGS.API
{
    /// <summary>
    /// A slider component allows selecting a numeric value between an allowed range, by dragging a handle across
    /// a line (usually).
    /// </summary>
	[RequiredComponent(typeof(ICollider))]
	[RequiredComponent(typeof(IDrawableInfo))]
	[RequiredComponent(typeof(IInObjectTree))]
	[RequiredComponent(typeof(IVisibleComponent))]
	[RequiredComponent(typeof(IEnabledComponent))]
	public interface ISliderComponent : IComponent
	{
        /// <summary>
        /// Gets or sets the graphics of the slider (on which the handle will be dragged).
        /// </summary>
        /// <value>The graphics.</value>
		IObject Graphics { get; set; }

        /// <summary>
        /// Gets or sets the handle graphics (the handle is the object the user drags to change the value).
        /// </summary>
        /// <value>The handle graphics.</value>
		IObject HandleGraphics { get; set; }

        /// <summary>
        /// Gets or sets an optional label which shows the value as text.
        /// </summary>
        /// <value>The label.</value>
		ILabel Label { get; set; }

        /// <summary>
        /// Gets or sets the allowed minimum value.
        /// </summary>
        /// <value>The minimum value.</value>
		float MinValue { get; set; }

        /// <summary>
        /// Gets or sets the allowed maximum value.
        /// </summary>
        /// <value>The max value.</value>
		float MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the slider's value.
        /// </summary>
        /// <value>The value.</value>
		float Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ISliderComponent"/> is horizontal (or vertical).
        /// </summary>
        /// <value><c>true</c> if is horizontal; otherwise, <c>false</c>.</value>
		bool IsHorizontal { get; set; }

        /// <summary>
        /// An event which is triggered when the slider value has changed.
        /// </summary>
        /// <value>The on value changed.</value>
		IEvent<SliderValueEventArgs> OnValueChanged { get; }
	}
}

