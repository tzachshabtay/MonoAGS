namespace AGS.API
{
    /// <summary>
    /// A string tree node, which will be shown in a <see cref="ITreeViewComponent"/> .
    /// </summary>
    public interface ITreeStringNode : IInTree<ITreeStringNode>
    {
        /// <summary>
        /// Gets or sets the text of the node.
        /// </summary>
        /// <value>The text.</value>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the text rendering configuration for the node.
        /// </summary>
        /// <value>The idle text config.</value>
        ITextConfig IdleTextConfig { get; set; }

        /// <summary>
        /// Gets or sets the text rendering configuration for the node when the mouse is hovered over it.
        /// </summary>
        /// <value>The hover text config.</value>
        ITextConfig HoverTextConfig { get; set; }

        /// <summary>
        /// Custom properties which can be assigned to the node and give more context.
        /// </summary>
        /// <value>The tag.</value>
        ICustomProperties Properties { get; }
    }
}
