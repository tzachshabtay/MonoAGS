using System;
using System.Collections.Generic;

namespace AGS.API
{
    [RequiredComponent(typeof(IInObjectTree))]    
    public interface IComboBoxComponent : IComponent
    {
        object SelectedItem { get; }
        int SelectedIndex { get; set; }
        IList<object> Items { get; }

        IPanel DropDownPanel { get; }
        ITextbox Textbox { get; set; }
        IButton DropDownButton { get; set; }
        Func<IButton> ItemButtonFactory { get; set; }
        IEnumerable<IButton> ItemButtons { get; }
    }
}
