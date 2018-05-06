using AGS.API;

namespace AGS.Engine
{
    public class AGSTreeStringNode : AGSComponent, ITreeStringNode
    {
        private readonly IStringItem _item;

        public AGSTreeStringNode(string text, IFont font)
        {
            TreeNode = new AGSTreeNode<ITreeStringNode>(this);
            _item = new AGSStringItem(font);
            Text = text;
        }

        public ITextConfig HoverTextConfig { get => _item.HoverTextConfig; set => _item.HoverTextConfig = value; }

        public ITextConfig IdleTextConfig { get => _item.IdleTextConfig; set => _item.IdleTextConfig = value; }

        public string Text { get => _item.Text; set => _item.Text = value; }

        public ICustomProperties Properties => _item.Properties;

        public ITreeNode<ITreeStringNode> TreeNode { get; }
    }
}
