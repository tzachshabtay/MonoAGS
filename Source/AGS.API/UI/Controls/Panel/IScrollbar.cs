using System;
namespace AGS.API
{
    /// <summary>
    /// A scrollbar is a <see cref="ISlider"/> with 2 buttons (left + right or up + down depending on which <see cref="SliderDirection"/> is used) that can
    /// be clicked to modify the slider.
    /// </summary>
    public interface IScrollbar
    {
        /// <summary>
        /// The up button (if the slider is vertical), or left button (if the slider is horizontal).
        /// </summary>
        /// <value>Up button.</value>
        IButton UpButton { get; }

        /// <summary>
        /// The down button (if the slider is vertical), or right button (if the slider is horizontal).
        /// </summary>
        /// <value>Down button.</value>
        IButton DownButton { get; }

        /// <summary>
        /// Gets the slider.
        /// </summary>
        /// <value>The slider.</value>
        ISlider Slider { get; }

        /// <summary>
        /// Gets or sets the step- the amount to move the slider when clicking the buttons.
        /// </summary>
        /// <value>The step.</value>
        float Step { get; set; }
    }
}