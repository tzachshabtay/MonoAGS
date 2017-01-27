namespace AGS.Engine.Android
{
    public class AndroidKeyboardState : IKeyboardState
    {
        public bool CapslockOn { get { return AndroidGameWindow.Instance.View.CapslockOn; } }
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
