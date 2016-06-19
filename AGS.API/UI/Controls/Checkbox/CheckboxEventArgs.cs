namespace AGS.API
{
    public class CheckboxEventArgs : AGSEventArgs
    {
        public CheckboxEventArgs(bool isChecked)
        {
            Checked = isChecked;
        }

        public bool Checked { get; private set; }
    }
}
