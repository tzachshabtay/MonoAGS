using System;
using AGS.API;

namespace AGS.Engine
{
    public class InspectorTreeNode : AGSComponent, IInspectorTreeNode
    {
		private readonly IStringItem _item;

		public InspectorTreeNode(InspectorProperty property, IInspectorPropertyEditor editor)
		{
			TreeNode = new AGSTreeNode<ITreeStringNode>(this);
			_item = new AGSStringItem();
            Text = property.Name;
            Property = property;
            Editor = editor;
		}

		public ITextConfig HoverTextConfig { get { return _item.HoverTextConfig; } set { _item.HoverTextConfig = value; } }

		public ITextConfig IdleTextConfig { get { return _item.IdleTextConfig; } set { _item.IdleTextConfig = value; } }

		public string Text { get { return _item.Text; } set { _item.Text = value; } }

		public ICustomProperties Properties { get { return _item.Properties; } }

		public ITreeNode<ITreeStringNode> TreeNode { get; private set; }

        public InspectorProperty Property { get; private set; }

        public IInspectorPropertyEditor Editor { get; private set; }
    }
}
