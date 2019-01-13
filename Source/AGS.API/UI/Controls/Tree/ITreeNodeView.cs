namespace AGS.API
{
    /// <summary>
    /// Encapsulates all the UI controls needed to show a tree node in a <see cref="ITreeViewComponent"/>.
    /// Each node has a parent panel with two additional panels: a horizontal stack layout panel
    /// with an expand/collapse button and the text label, and a vertical panel for holding the children
    /// of the node.
    /// </summary>
    public interface ITreeNodeView
    {
        /// <summary>
        /// Gets the text label for the node.
        /// </summary>
        /// <value>The tree item.</value>
        IUIControl TreeItem { get; }

        /// <summary>
        /// Gets the expand/collapse button for the node.
        /// </summary>
        /// <value>The expand button.</value>
        IButton ExpandButton { get; }

        /// <summary>
        /// Gets the parent panel which contains all of the controls in the node.
        /// </summary>
        /// <value>The parent panel.</value>
        IPanel ParentPanel { get; }

        /// <summary>
        /// Gets the vertical panel which contains all of the children of the node.
        /// </summary>
        /// <value>The vertical panel.</value>
        IPanel VerticalPanel { get; }

        /// <summary>
        /// Gets the horizontal panel which contains the expand/collapse button and the text label.
        /// </summary>
        /// <value>The horizontal panel.</value>
        IPanel HorizontalPanel { get; }

        /// <summary>
        /// An event that can be triggered to notify the tree view that this node needs to be re-rendered on screen.
        /// </summary>
        /// <value>The on refresh display needed event.</value>
        IBlockingEvent OnRefreshDisplayNeeded { get; }
    }
}
