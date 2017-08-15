using AGS.API;

namespace AGS.Engine
{
    public class AGSTreeStringNode : AGSComponent, ITreeStringNode
    {
        private readonly IStringItem _item;

        public AGSTreeStringNode()
        {
            TreeNode = new AGSTreeNode<ITreeStringNode>(this);
            _item = new AGSStringItem();
        }

        public ITextConfig HoverTextConfig { get { return _item.HoverTextConfig; } set { _item.HoverTextConfig = value; } }

        public ITextConfig IdleTextConfig { get { return _item.IdleTextConfig; } set { _item.IdleTextConfig = value; } }

        public string Text { get { return _item.Text; } set { _item.Text = value; } }

        public ICustomProperties Properties { get { return _item.Properties; } }

        public ITreeNode<ITreeStringNode> TreeNode { get; private set; }
    }
}
