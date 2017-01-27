using System;

namespace AGS.Engine.Desktop
{
    public class DesktopKeyboardState : IKeyboardState
    {
        public bool CapslockOn { get { return Console.CapsLock; } }
        public bool SoftKeyboardVisible { get { return false; } }

        public void HideSoftKeyboard() { }

        public void ShowSoftKeyboard() { }
    }
}
