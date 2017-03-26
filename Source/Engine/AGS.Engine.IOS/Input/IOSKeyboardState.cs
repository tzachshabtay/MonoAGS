namespace AGS.Engine.IOS
{
    public class IOSKeyboardState : IKeyboardState
    {
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
    }
}
