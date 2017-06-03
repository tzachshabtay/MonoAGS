namespace AGS.API
{
    public interface ITreeNodeViewProvider
    {
        ITreeNodeView CreateNode(ITreeStringNode node);

        void BeforeDisplayingNode(ITreeStringNode item, ITreeNodeView nodeView, bool isCollapsed, bool isHovered);
    }
}
