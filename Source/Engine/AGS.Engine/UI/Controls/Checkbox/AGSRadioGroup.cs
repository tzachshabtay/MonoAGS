using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
	public class AGSRadioGroup : IRadioGroup
    {
        public ICheckboxComponent SelectedButton { get; set; }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}