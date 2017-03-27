using System;
using AGS.API;

namespace AGS.Engine.Android
{
    public class AndroidKeyboardState : IKeyboardState
    {
        public AndroidKeyboardState()
        {
            OnSoftKeyboardHidden = new AGSEvent<AGSEventArgs>();
        }

        public bool CapslockOn { get { return AndroidGameWindow.Instance.View.CapslockOn; } }

        public IEvent<AGSEventArgs> OnSoftKeyboardHidden { get; private set; }

        public bool SoftKeyboardVisible { get { return AndroidGameWindow.Instance.View.SoftKeyboardVisible; } }

        public void HideSoftKeyboard()
        {
            AndroidGameWindow.Instance.View.HideKeyboard();
        }

        public void ShowSoftKeyboard()
        {
            AndroidGameWindow.Instance.View.ShowKeyboard();
        }
    }
}
