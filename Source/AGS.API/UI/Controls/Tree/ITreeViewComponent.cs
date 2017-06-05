namespace AGS.API
{
    public enum SelectionType
    {
        None,
        Single,
        //todo: support multiple selection
    }

    [RequiredComponent(typeof(IInObjectTree))]
    [RequiredComponent(typeof(IDrawableInfo))]
    public interface ITreeViewComponent : IComponent
    {
        ITreeStringNode Tree { get; set; }

        ITreeNodeViewProvider NodeViewProvider { get; set; }

        float HorizontalSpacing { get; set; }

        float VerticalSpacing { get; set; }

        SelectionType AllowSelection { get; set; }

        IEvent<NodeEventArgs> OnNodeSelected { get; }
    }
}
