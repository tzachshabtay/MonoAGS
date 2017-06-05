namespace AGS.API
{
    public interface ITreeNodeViewProvider
    {
        ITreeNodeView CreateNode(ITreeStringNode node, IRenderLayer layer);

        void BeforeDisplayingNode(ITreeStringNode item, ITreeNodeView nodeView, 
                                  bool isCollapsed, bool isHovered, bool isSelected);
    }
}
