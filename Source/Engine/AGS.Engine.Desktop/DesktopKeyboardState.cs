using System;
using AGS.API;

namespace AGS.Engine.Desktop
{
    public class DesktopKeyboardState : IKeyboardState
    {
        public DesktopKeyboardState()
        {
            OnSoftKeyboardHidden = new AGSEvent();
        }

        public bool CapslockOn => Console.CapsLock;

        public IEvent OnSoftKeyboardHidden { get; }

        public bool SoftKeyboardVisible => false;

        public void HideSoftKeyboard() { }

        public void ShowSoftKeyboard() { }
    }
}
