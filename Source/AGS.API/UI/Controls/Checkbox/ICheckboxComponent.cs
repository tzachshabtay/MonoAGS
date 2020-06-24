﻿namespace AGS.API
{
    /// <summary>
    /// The checkbox component allows having an entity behave like a checkbox (can be checked an unchecked).
    /// </summary>
    [RequiredComponent(typeof(IUIEvents))]
    [RequiredComponent(typeof(IAnimationComponent))]
    [RequiredComponent(typeof(ITextComponent), false)]
    [RequiredComponent(typeof(IImageComponent), false)]
    [RequiredComponent(typeof(IBorderComponent), false)]
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
        IBlockingEvent<CheckBoxEventArgs> OnCheckChanged { get; }

        /// <summary>
        /// Gets or sets the animation when the checkbox is not checked.
        /// </summary>
        /// <value>The not checked animation.</value>
        ButtonAnimation NotCheckedAnimation { get; set; }

        /// <summary>
        /// Gets or sets the animation when the checkbox is not checked and the mouse is hovering over it.
        /// </summary>
        /// <value>The hover not checked animation.</value>
        ButtonAnimation HoverNotCheckedAnimation { get; set; }

        /// <summary>
        /// Gets or sets the animation when the checkbox is checked.
        /// </summary>
        /// <value>The checked animation.</value>
        ButtonAnimation CheckedAnimation { get; set; }

        /// <summary>
        /// Gets or sets the animation when the checkbox is checked and the mouse is hovering over it.
        /// </summary>
        /// <value>The hover checked animation.</value>
        ButtonAnimation HoverCheckedAnimation { get; set; }

        /// <summary>
        /// An optional text label to accompany the checkbox.
        /// </summary>
        /// <value>The text label.</value>
        ILabel TextLabel { get; set; }

        /// <summary>
        /// Allows setting a radio group, which can be used to group multiple checkboxes together to guarantee that 
        /// only a single checkbox is checked at any given time.
        /// </summary>
        /// <value>The radio group.</value>
        IRadioGroup RadioGroup { get; set; }
    }
}
