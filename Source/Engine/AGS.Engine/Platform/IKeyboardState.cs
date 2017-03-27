using AGS.API;

namespace AGS.Engine
{
    public interface IKeyboardState
    {
        IEvent<AGSEventArgs> OnSoftKeyboardHidden { get; }
        bool CapslockOn { get; }
        bool SoftKeyboardVisible { get; }
        void ShowSoftKeyboard();
        void HideSoftKeyboard();
    }
}
