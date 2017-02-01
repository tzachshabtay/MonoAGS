using System;
namespace AGS.Engine.IOS
{
    public class IOSKeyboardState : IKeyboardState
    {
        public IOSKeyboardState()
        {
        }

        public bool CapslockOn { get { return false; } }

        public bool SoftKeyboardVisible { get { return false; } }

        public void HideSoftKeyboard()
        {
        }

        public void ShowSoftKeyboard()
        {
        }
    }
}
