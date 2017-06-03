namespace AGS.API
{
    public interface ITreeViewComponent : IComponent
    {
        ITreeStringNode Tree { get; set; }

        ITreeNodeViewProvider NodeViewProvider { get; set; }

        float HorizontalSpacing { get; set; }

        float VerticalSpacing { get; set; }
    }
}
