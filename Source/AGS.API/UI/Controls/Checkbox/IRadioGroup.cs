using System.ComponentModel;

namespace AGS.API
{
    /// <summary>
    /// Used to group multiple checkboxes together to guarantee that only one will be checked at any given time.
    /// </summary>
    /// <seealso cref="ICheckboxComponent.RadioGroup"/>
    public interface IRadioGroup : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the selected button.
        /// Once a new button is selected, any existing checked button in the group will be unchecked.
        /// </summary>
        /// <value>The selected button.</value>
        ICheckboxComponent SelectedButton { get; set; }
    }
}