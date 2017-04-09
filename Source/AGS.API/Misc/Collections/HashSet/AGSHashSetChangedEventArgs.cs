namespace AGS.API
{
    public class AGSHashSetChangedEventArgs<TItem> : AGSEventArgs
    {
        public AGSHashSetChangedEventArgs(ListChangeType changeType, TItem item)
        {
            ChangeType = changeType;
            Item = item;
        }

        public ListChangeType ChangeType { get; private set; }
        public TItem Item { get; private set; }
    }
}
