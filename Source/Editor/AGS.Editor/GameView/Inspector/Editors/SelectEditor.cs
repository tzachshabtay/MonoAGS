using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class SelectEditor : IInspectorPropertyEditor
    {
        private readonly IUIFactory _factory;
        private readonly ActionManager _actions;
        private readonly StateModel _model;
        private IProperty _property;
        private ITextComponent _text;
        private readonly Func<IProperty, List<IStringItem>> _getOptions;
        private readonly Func<IStringItem, IProperty, Action, Task<ReturnValue>> _getValue;

        public class ReturnValue
        {
            public ReturnValue(object value, bool shouldCancel)
            {
                Value = value;
                ShouldCancel = shouldCancel;
            }

            public object Value { get; }
            public bool ShouldCancel { get; }
        }

        public SelectEditor(IUIFactory factory, ActionManager actions, StateModel model, 
                            Func<IProperty, List<IStringItem>> getOptions, Func<IStringItem, IProperty, Action, Task<ReturnValue>> getValue)
        {
            _factory = factory;
            _actions = actions;
            _model = model;
            _getOptions = getOptions;
            _getValue = getValue;
        }

        public void AddEditorUI(string id, ITreeNodeView view, IProperty property)
        {
            _property = property;
            var label = view.TreeItem;
            var combobox = _factory.GetComboBox(id, null, null, null, label.TreeNode.Parent, defaultWidth: 100f, defaultHeight: 25f);
            _text = combobox.TextBox;
            _text.TextBackgroundVisible = false;
            var list = _getOptions(property);
            combobox.DropDownPanelList.Items.AddRange(list);
            combobox.Z = label.Z;
            combobox.TextBox.Text = property.ValueString;
            combobox.TextBox.TextConfig.AutoFit = AutoFit.LabelShouldFitText;
            if (list.Count > 5) //If more than 5 items in the dropdown, let's have it with textbox suggestions as user might prefer to type for filtering the dropdown
            {
                combobox.SuggestMode = ComboSuggest.Enforce;
            }
            Action closeCombobox = () => (combobox.DropDownPanel.ScrollingPanel ?? combobox.DropDownPanel.ContentsPanel).Visible = false;
            combobox.DropDownPanelList.OnSelectedItemChanging.SubscribeToAsync(async args =>
            {
                if (_actions.ActionIsExecuting) return;
                var returnValue = await _getValue(args.Item, property, closeCombobox);
                if (returnValue.ShouldCancel)
                {
                    args.ShouldCancel = true;
                    return;
                }
                _actions.RecordAction(new PropertyAction(property, returnValue.Value, _model));
            });
        }

        public void RefreshUI()
        {
            if (_text == null) return;
            _text.Text = _property.ValueString;
        }
    }
}
