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
	[RequiredComponent(typeof(IDrawableInfoComponent))]
	[RequiredComponent(typeof(IInObjectTreeComponent))]
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
        /// Note that the value will be clamped to <see cref="MinValue"/> and <see cref="MaxValue"/> (in other words, if you try to set a value above the maximum it will set it to the maximum, and the same for the minimum value).
        /// </summary>
        /// <value>The value.</value>
		float Value { get; set; }

        /// <summary>
        /// Gets or sets an offset for the handle to be placed from the minimum X/Y of the background image.
        /// This can be useful for properly aligning the handle to the background graphics.
        /// </summary>
        /// <value>The minimum handle offset.</value>
        float MinHandleOffset { get; set; }

        /// <summary>
        /// Gets or sets an offset for the handle to be placed from the maximum X/Y of the background image.
        /// This can be useful for properly aligning the handle to the background graphics.
        /// </summary>
        /// <value>The minimum handle offset.</value>
        float MaxHandleOffset { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ISliderComponent"/> allows keyboard control for moving the slider when it is in focus (i.e after the user clicked on the slider).
        /// Default is true.
        /// </summary>
        /// <value><c>true</c> if allow keyboard control; otherwise, <c>false</c>.</value>
        bool AllowKeyboardControl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ISliderComponent"/> should clamp the slider value
        /// to the minimum and/or maximum when those change. This is enabled by default.
        /// For example, if the slider value is 100 and I updated the maximum from 200 to 50, if this property is enabled,
        /// the value will automatically change to 50. If the property is disabled, the value will stay at 100.
        /// </summary>
        /// <value><c>true</c> if should clamp values when changing minimum max; otherwise, <c>false</c>.</value>
        bool ShouldClampValuesWhenChangingMinMax { get; set; }

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

        /// <summary>
        /// Increase the the slider by the specified amount.
        /// This checks for a case where the value is below the minimum (could happen when <see cref="ShouldClampValuesWhenChangingMinMax"/> is false), and 
        /// if true it increases the value from the minimum and not from the current value.
        /// </summary>
        /// <param name="step">Step.</param>
        void Increase(float step);

        /// <summary>
        /// Decrease the the slider by the specified amount.
        /// This checks for a case where the value is above the maximum (could happen when <see cref="ShouldClampValuesWhenChangingMinMax"/> is false), and 
        /// if true it decreases the value from the maximum and not from the current value.
        /// </summary>
        /// <param name="step">Step.</param>
        void Decrease(float step);
	}
}