using System;
using AGS.API;

namespace AGS.Engine.Desktop
{
    public class DesktopKeyboardState : IKeyboardState
    {
        public DesktopKeyboardState()
        {
            OnSoftKeyboardHidden = new AGSEvent<object>();
        }

        public bool CapslockOn { get { return Console.CapsLock; } }

        public IEvent<object> OnSoftKeyboardHidden { get; private set; }

        public bool SoftKeyboardVisible { get { return false; } }

        public void HideSoftKeyboard() { }

        public void ShowSoftKeyboard() { }
    }
}
