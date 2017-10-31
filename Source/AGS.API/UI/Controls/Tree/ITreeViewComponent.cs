namespace AGS.API
{
    public enum SelectionType
    {
        None,
        Single,
        //todo: support multiple selection
    }

    /// <summary>
    /// A component for displaying a hierarchical collection of text labels.
    /// Each label is a node in the tree which can be collapsed/expanded to hide/show its children.
    /// </summary>
    [RequiredComponent(typeof(IInObjectTree))]
    [RequiredComponent(typeof(IDrawableInfo))]
    public interface ITreeViewComponent : IComponent
    {
        /// <summary>
        /// The tree of text items to show.
        /// </summary>
        /// <value>The tree.</value>
        ITreeStringNode Tree { get; set; }

        /// <summary>
        /// Gets or sets the node view provider, which is responsible for displaying a node in the tree.
        /// </summary>
        /// <value>The node view provider.</value>
        ITreeNodeViewProvider NodeViewProvider { get; set; }

        /// <summary>
        /// Gets or sets the horizontal spacing between each level in the tree.
        /// </summary>
        /// <value>The horizontal spacing.</value>
        float HorizontalSpacing { get; set; }

        /// <summary>
        /// Gets or sets the vertical spacing between each child in the tree.
        /// </summary>
        /// <value>The vertical spacing.</value>
        float VerticalSpacing { get; set; }

        /// <summary>
        /// Gets or sets whether to allow selecting nodes in the tree.
        /// </summary>
        /// <value>The allow selection.</value>
        SelectionType AllowSelection { get; set; }

        /// <summary>
        /// An event which fires every time a node is selected (if <see cref="AllowSelection"/> is set to allow selection).
        /// </summary>
        IEvent<NodeEventArgs> OnNodeSelected { get; }

		/// <summary>
		/// An event which fires every time a node is expanded.
		/// </summary>
		IEvent<NodeEventArgs> OnNodeExpanded { get; }

		/// <summary>
		/// An event which fires every time a node is expanded.
		/// </summary>
		IEvent<NodeEventArgs> OnNodeCollapsed { get; }

        /// <summary>
        /// Expand the specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        void Expand(ITreeStringNode node);

        /// <summary>
        /// Collapse the specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        void Collapse(ITreeStringNode node);

        /// <summary>
        /// Forces a layout refresh for the tree (this usually should not be necessary, as the tree refreshes itself when it sees a need).
        /// </summary>
        void RefreshLayout();
    }
}
