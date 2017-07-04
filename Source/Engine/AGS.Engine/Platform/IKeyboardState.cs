using AGS.API;

namespace AGS.Engine
{
    public interface IKeyboardState
    {
        IEvent<object> OnSoftKeyboardHidden { get; }
        bool CapslockOn { get; }
        bool SoftKeyboardVisible { get; }
        void ShowSoftKeyboard();
        void HideSoftKeyboard();
    }
}
