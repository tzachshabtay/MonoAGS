namespace AGS.Engine
{
    public interface IKeyboardState
    {
        bool CapslockOn { get; }
        bool SoftKeyboardVisible { get; }
        void ShowSoftKeyboard();
        void HideSoftKeyboard();
    }
}
