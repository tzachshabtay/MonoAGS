using System;
using System.Collections.Generic;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class EnumPropertyEditor : IInspectorPropertyEditor
    {
        private readonly IUIFactory _factory;
        private readonly ActionManager _actions;
        private readonly StateModel _model;
        private IProperty _property;
        private ITextComponent _text;

        public EnumPropertyEditor(IUIFactory factory, ActionManager actions, StateModel model)
        {
            _factory = factory;
            _actions = actions;
            _model = model;
        }

        public void AddEditorUI(string id, ITreeNodeView view, IProperty property)
        {
            _property = property;
            var label = view.TreeItem;
            var combobox = _factory.GetComboBox(id, null, null, null, label.TreeNode.Parent, defaultWidth: 100f, defaultHeight: 25f);
            _text = combobox.TextBox;
            _text.TextBackgroundVisible = false;
            var list = new List<IStringItem>();
            Type enumType = property.PropertyType;
            foreach (var option in Enum.GetValues(enumType))
			{
				list.Add(new AGSStringItem { Text = option.ToString() });
			}
            combobox.DropDownPanelList.Items.AddRange(list);
            combobox.Z = label.Z;
            combobox.TextBox.Text = property.ValueString;
            combobox.TextBox.TextConfig.AutoFit = AutoFit.LabelShouldFitText;
            if (list.Count > 5) //If more than 5 items in the dropdown, let's have it with textbox suggestions as user might prefer to type for filtering the dropdown
            {
                combobox.SuggestMode = ComboSuggest.Enforce;
            }
            combobox.DropDownPanelList.OnSelectedItemChanged.Subscribe(args => 
            {
                if (_actions.ActionIsExecuting) return;
                _actions.RecordAction(new PropertyAction(property, Enum.Parse(enumType, args.Item.Text), _model));
            });
        }

        public void RefreshUI()
        {
            if (_text == null) return;
            _text.Text = _property.ValueString;
        }
    }
}
