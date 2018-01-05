using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class EnumPropertyEditor : IInspectorPropertyEditor
    {
        private readonly IUIFactory _factory;
        private InspectorProperty _property;
        private ITextComponent _text;

        public EnumPropertyEditor(IUIFactory factory)
        {
            _factory = factory;
        }

        public void AddEditorUI(string id, ITreeNodeView view, InspectorProperty property)
        {
            _property = property;
            var label = view.TreeItem;
            var combobox = _factory.GetComboBox(id, null, null, null, label.TreeNode.Parent, defaultWidth: 100f, defaultHeight: 25f);
            _text = combobox.TextBox;
            _text.TextBackgroundVisible = false;
            var list = new List<IStringItem>();
            Type enumType = property.Prop.PropertyType;
            foreach (var option in Enum.GetValues(enumType))
			{
				list.Add(new AGSStringItem { Text = option.ToString() });
			}
            combobox.DropDownPanelList.Items.AddRange(list);
            combobox.Z = label.Z;
            combobox.TextBox.Text = property.Value;
            combobox.TextBox.TextConfig.AutoFit = AutoFit.LabelShouldFitText;
            combobox.DropDownPanelList.OnSelectedItemChanged.Subscribe(args => 
            {
                property.Prop.SetValue(property.Object, Enum.Parse(enumType, args.Item.Text));
            });
        }

        public void RefreshUI()
        {
            if (_text == null) return;
            _text.Text = _property.Value;
        }
    }
}
