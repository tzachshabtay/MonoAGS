namespace AGS.API
{
    public class NodeEventArgs : AGSEventArgs
    {
        public NodeEventArgs(ITreeStringNode node)
        {
            Node = node;
        }

        public ITreeStringNode Node { get; private set; }
    }
}
