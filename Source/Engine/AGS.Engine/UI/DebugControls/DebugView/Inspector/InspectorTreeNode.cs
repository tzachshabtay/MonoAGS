using System;
using AGS.API;

namespace AGS.Engine
{
    public class InspectorTreeNode : AGSComponent, IInspectorTreeNode
    {
		private readonly IStringItem _item;

		public InspectorTreeNode(string text, string value)
		{
			TreeNode = new AGSTreeNode<ITreeStringNode>(this);
			_item = new AGSStringItem();
            Text = text;
            Value = value;
		}

		public ITextConfig HoverTextConfig { get { return _item.HoverTextConfig; } set { _item.HoverTextConfig = value; } }

		public ITextConfig IdleTextConfig { get { return _item.IdleTextConfig; } set { _item.IdleTextConfig = value; } }

		public string Text { get { return _item.Text; } set { _item.Text = value; } }

		public ICustomProperties Properties { get { return _item.Properties; } }

		public ITreeNode<ITreeStringNode> TreeNode { get; private set; }

        public string Value { get; private set; }
    }
}
