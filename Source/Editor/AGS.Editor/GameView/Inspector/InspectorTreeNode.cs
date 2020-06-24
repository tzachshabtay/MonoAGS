﻿using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class InspectorTreeNode : AGSComponent, IInspectorTreeNode, ICustomSearchItem
    {
		private readonly IStringItem _item;

		public InspectorTreeNode(IProperty property, IInspectorPropertyEditor editor, IFont font, bool isCategory)
		{
			TreeNode = new AGSTreeNode<ITreeStringNode>(this);
			_item = new AGSStringItem(font);
            Text = property.DisplayName;
            Property = property;
            Editor = editor;
            IsCategory = isCategory;
        }

        public bool IsCategory { get; }

		public ITextConfig HoverTextConfig { get => _item.HoverTextConfig; set => _item.HoverTextConfig = value; }

		public ITextConfig IdleTextConfig { get => _item.IdleTextConfig; set => _item.IdleTextConfig = value; }

		public string Text { get => _item.Text; set => _item.Text = value; }

        public ICustomProperties Properties => _item.Properties;

        public ITreeNode<ITreeStringNode> TreeNode { get; }

        public IProperty Property { get; }

        public IInspectorPropertyEditor Editor { get; }

        public bool Contains(string searchText)
        {
            string text = Text?.ToLowerInvariant() ?? "";
            if (text.Contains(searchText)) return true;
            text = Property.ValueString?.ToLowerInvariant() ?? "";
            return text.Contains(searchText);
        }
    }
}