namespace AGS.API
{
    /// <summary>
    /// The direction in which the slider moves from minimum to maximum.
    /// </summary>
    public enum SliderDirection
    {
        /// <summary>
        /// The slider is vertical and moves from bottom (minimum value) to top (maximum value).
        /// </summary>
        BottomToTop,
        /// <summary>
        /// The slider is vertical moves from top (minimum value) to bottom (maximum value).
        /// </summary>
        TopToBottom,
        /// <summary>
        /// The slider is horizontal and moves from left (minimum value) to right (maximum value).
        /// </summary>
        LeftToRight,
        /// <summary>
        /// The slider is horizontal and moves from right (minimum value) to left (maximum value).
        /// </summary>
        RightToLeft,
    }

    /// <summary>
    /// A slider component allows selecting a numeric value between an allowed range, by dragging a handle across
    /// a line (usually).
    /// </summary>
    [RequiredComponent(typeof(IBoundingBoxComponent))]
	[RequiredComponent(typeof(IDrawableInfo))]
	[RequiredComponent(typeof(IInObjectTree))]
	[RequiredComponent(typeof(IVisibleComponent))]
	[RequiredComponent(typeof(IEnabledComponent))]
    [RequiredComponent(typeof(IUIEvents))]
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
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ISliderComponent"/> allows keyboard control for moving the slider when it is in focus (i.e after the user clicked on the slider).
        /// Default is true.
        /// </summary>
        /// <value><c>true</c> if allow keyboard control; otherwise, <c>false</c>.</value>
        bool AllowKeyboardControl { get; set; }

        /// <summary>
        /// Gets or sets the direction of the slider. If, for example, the slider's direction is bottom-to-top then the minimum value will correspond with the bottom most location on the slider
        /// and the maximum value will correspond with the top most location on the slider.
        /// </summary>
        /// <value><c>true</c> if is horizontal; otherwise, <c>false</c>.</value>
        SliderDirection Direction { get; set; }

        /// <summary>
        /// An event which is triggered when the slider value has changed.
        /// This only gets called after user finishes dragging the handle on the slider.
        /// For continous refreshes as the user drags the handle, <see cref="OnValueChanging"/>.
        /// </summary>
        /// <value>The on value changed.</value>
        IBlockingEvent<SliderValueEventArgs> OnValueChanged { get; }

        /// <summary>
        /// An event which is triggered when the slider value is changing.
        /// This will be fired continously as the user drags the handle on the slider.
        /// If you need an event which fires only when the user finishes dragging, <see cref="OnValueChanged"/>. 
        /// </summary>
        /// <value>The on value changed.</value>
        IBlockingEvent<SliderValueEventArgs> OnValueChanging { get; }
	}
}

