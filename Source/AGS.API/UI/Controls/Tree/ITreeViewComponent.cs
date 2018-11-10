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
    [RequiredComponent(typeof(IInObjectTreeComponent))]
    [RequiredComponent(typeof(IDrawableInfoComponent))]
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
        /// Gets or sets the search filter (a search text that filters the tree so that
        /// only items containing the text appear in the tree).
        /// </summary>
        /// <value>The search filter.</value>
        string SearchFilter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ITreeViewComponent"/> skip rendering the root node.
        /// This effectively means that the tree will show multiple "detached" roots (all the nodes in the second level in the tree),
        /// i.e it makes it possible for the tree view to show multiple "trees".
        /// </summary>
        /// <value><c>true</c> if skip rendering root; otherwise, <c>false</c>.</value>
        bool SkipRenderingRoot { get; set; }

        /// <summary>
        /// If the tree view is contained in a scrolling panel (<see cref="IUIFactory.CreateScrollingPanel"/>), you can set
        /// the containing contents panel here, which the tree view will then use to optimize performance by only loading the tree node views when 
        /// they're inside the scrolling range.
        /// </summary>
        /// <value>The scrolling container.</value>
        IEntity ScrollingContainer { get; set; }

        /// <summary>
        /// An event which fires every time a node is selected (if <see cref="AllowSelection"/> is set to allow selection).
        /// </summary>
        IBlockingEvent<NodeEventArgs> OnNodeSelected { get; }

		/// <summary>
		/// An event which fires every time a node is expanded.
		/// </summary>
        IBlockingEvent<NodeEventArgs> OnNodeExpanded { get; }

		/// <summary>
		/// An event which fires every time a node is expanded.
		/// </summary>
        IBlockingEvent<NodeEventArgs> OnNodeCollapsed { get; }

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
        /// Is the specified node collapsed (or expanded)?
        /// </summary>
        /// <returns>Null if the node is not in the tree, false if expanded, true if collapsed.</returns>
        /// <param name="node">Node.</param>
        bool? IsCollapsed(ITreeStringNode node);

        /// <summary>
        /// Select the specified node in the tree.
        /// </summary>
        /// <param name="node">Node.</param>
        void Select(ITreeStringNode node);

        /// <summary>
        /// Forces a layout refresh for the tree (this usually should not be necessary, as the tree refreshes itself when it sees a need).
        /// </summary>
        void RefreshLayout();
    }
}
