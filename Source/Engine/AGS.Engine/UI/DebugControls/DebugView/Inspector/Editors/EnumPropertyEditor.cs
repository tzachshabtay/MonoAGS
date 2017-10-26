using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class EnumPropertyEditor : IInspectorPropertyEditor
    {
        private readonly IUIFactory _factory;

        public EnumPropertyEditor(IUIFactory factory)
        {
            _factory = factory;
        }

        public void AddEditorUI(string id, ITreeNodeView view, InspectorProperty property)
        {
            var label = view.TreeItem;
            var combobox = _factory.GetComboBox(id, null, null, null, label.TreeNode.Parent, defaultWidth: 100f, defaultHeight: 25f);
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
    }
}
