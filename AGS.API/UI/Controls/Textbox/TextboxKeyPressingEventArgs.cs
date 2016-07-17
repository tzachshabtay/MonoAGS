namespace AGS.API
{
    public class TextBoxKeyPressingEventArgs : AGSEventArgs
    {
        public TextBoxKeyPressingEventArgs(Key pressedKey)
        {
            PressedKey = pressedKey;
        }

        public Key PressedKey { get; private set; }
        public bool ShouldCancel { get; set; }
    }
}
