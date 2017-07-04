namespace AGS.API
{
    /// <summary>
    /// Event arguments which contain a tree node.
    /// </summary>
    public class NodeEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.NodeEventArgs"/> class.
        /// </summary>
        /// <param name="node">Node.</param>
        public NodeEventArgs(ITreeStringNode node)
        {
            Node = node;
        }

        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <value>The node.</value>
        public ITreeStringNode Node { get; private set; }
    }
}
