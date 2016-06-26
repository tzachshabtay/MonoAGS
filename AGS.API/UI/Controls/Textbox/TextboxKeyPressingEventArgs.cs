namespace AGS.API
{
    public class TextboxKeyPressingEventArgs : AGSEventArgs
    {
        public TextboxKeyPressingEventArgs(Key pressedKey)
        {
            PressedKey = pressedKey;
        }

        public Key PressedKey { get; private set; }
        public bool ShouldCancel { get; set; }
    }
}
