﻿using System;
using AGS.API;

namespace AGS.Engine.Android
{
    public class AndroidKeyboardState : IKeyboardState
    {
        public AndroidKeyboardState()
        {
            OnSoftKeyboardHidden = new AGSEvent();
        }

        public bool CapslockOn => AndroidGameWindow.Instance.View.CapslockOn;

        public IEvent OnSoftKeyboardHidden { get; }

        public bool SoftKeyboardVisible => AndroidGameWindow.Instance.View.SoftKeyboardVisible;

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
