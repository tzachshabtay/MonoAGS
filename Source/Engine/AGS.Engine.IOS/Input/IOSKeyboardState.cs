extern alias IOS;

using AGS.API;
using IOS::Foundation;
using IOS::UIKit;

namespace AGS.Engine.IOS
{
    public class IOSKeyboardState : IKeyboardState
    {
        public IOSKeyboardState()
        {
            OnSoftKeyboardHidden = new AGSEvent();
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, onKeyboardHidden);
        }

        public IEvent OnSoftKeyboardHidden { get; private set; }

        public bool CapslockOn { get { return false; } }

        public bool SoftKeyboardVisible { get { return IOSGameWindow.Instance.View.IsFirstResponder; } }

        public void HideSoftKeyboard()
        {
            IOSGameWindow.Instance.View.BeginInvokeOnMainThread(() => IOSGameWindow.Instance.View.ResignFirstResponder()); 
        }

        public void ShowSoftKeyboard()
        {
            IOSGameWindow.Instance.View.HasText = false;
            IOSGameWindow.Instance.View.BeginInvokeOnMainThread(() => IOSGameWindow.Instance.View.BecomeFirstResponder());
        }

        private async void onKeyboardHidden(NSNotification obj)
        {
            var onSoftKeyboardHidden = OnSoftKeyboardHidden;
            if (onSoftKeyboardHidden != null) await onSoftKeyboardHidden.InvokeAsync();
        }
    }
}
