namespace AGS.API
{
    public class CheckBoxEventArgs : AGSEventArgs
    {
        public CheckBoxEventArgs(bool isChecked)
        {
            Checked = isChecked;
        }

        public bool Checked { get; private set; }
    }
}
