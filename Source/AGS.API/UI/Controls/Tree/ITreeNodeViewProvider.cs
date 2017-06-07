namespace AGS.API
{
    /// <summary>
    /// The tree node view provider is responsible for displaying a node in a <see cref="ITreeViewComponent"/> control.
    /// </summary>
    public interface ITreeNodeViewProvider
    {
        /// <summary>
        /// Creates a UI view for the text node.
        /// </summary>
        /// <returns>The node view.</returns>
        /// <param name="node">The text node.</param>
        /// <param name="layer">The rendering layer which all of the tree UI controls should use.</param>
        ITreeNodeView CreateNode(ITreeStringNode node, IRenderLayer layer);

        /// <summary>
        /// Called before displaying the node view, and allows for changing the display
        /// based on the current state of the node.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="nodeView">Node view.</param>
        /// <param name="isCollapsed">If set to <c>true</c> is collapsed.</param>
        /// <param name="isHovered">If set to <c>true</c> is hovered.</param>
        /// <param name="isSelected">If set to <c>true</c> is selected.</param>
        void BeforeDisplayingNode(ITreeStringNode item, ITreeNodeView nodeView, 
                                  bool isCollapsed, bool isHovered, bool isSelected);
    }
}
