namespace AGS.API
{
    public interface ITreeNodeView
    {
        ILabel TreeItem { get; }
        IButton ExpandButton { get; }
        IPanel ParentPanel { get; }
        IPanel VerticalPanel { get; }
    }
}
