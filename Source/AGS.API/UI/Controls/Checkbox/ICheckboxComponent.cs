namespace AGS.API
{
    /// <summary>
    /// The checkbox component allows having an entity behave like a checkbox (can be checked an unchecked).
    /// </summary>
    [RequiredComponent(typeof(IUIEvents))]
    [RequiredComponent(typeof(IAnimationContainer))]
    public interface ICheckboxComponent : IComponent
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ICheckboxComponent"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        bool Checked { get; set; }

        /// <summary>
        /// An event which is triggered whenever the check state changes.
        /// </summary>
        /// <value>The event.</value>
        IEvent<CheckBoxEventArgs> OnCheckChanged { get; }

        /// <summary>
        /// Gets or sets the animation when the checkbox is not checked.
        /// </summary>
        /// <value>The not checked animation.</value>
        IAnimation NotCheckedAnimation { get; set; }

        /// <summary>
        /// Gets or sets the animation when the checkbox is not checked and the mouse is hovering over it.
        /// </summary>
        /// <value>The hover not checked animation.</value>
        IAnimation HoverNotCheckedAnimation { get; set; }

        /// <summary>
        /// Gets or sets the animation when the checkbox is checked.
        /// </summary>
        /// <value>The checked animation.</value>
        IAnimation CheckedAnimation { get; set; }

        /// <summary>
        /// Gets or sets the animation when the checkbox is checked and the mouse is hovering over it.
        /// </summary>
        /// <value>The hover checked animation.</value>
        IAnimation HoverCheckedAnimation { get; set; }
    }
}
