using AGS.API;

namespace AGS.Engine
{
    public interface IKeyboardState
    {
        IEvent OnSoftKeyboardHidden { get; }
        bool CapslockOn { get; }
        bool SoftKeyboardVisible { get; }
        void ShowSoftKeyboard();
        void HideSoftKeyboard();
    }
}
