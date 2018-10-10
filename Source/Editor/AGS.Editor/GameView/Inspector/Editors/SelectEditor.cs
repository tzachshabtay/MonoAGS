using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AGS.API;
using GuiLabs.Undo;
using Humanizer;

namespace AGS.Editor
{
    public class SelectEditor : IInspectorPropertyEditor
    {
        private readonly IGameFactory _factory;
        private readonly ActionManager _actions;
        private readonly StateModel _model;
        private IProperty _property;
        private ITextComponent _text;
        private readonly Func<IProperty, List<IStringItem>> _getOptions;
        private readonly Func<IStringItem, IProperty, Action, Task<ReturnValue>> _getValue;

        public class ReturnValue
        {
            public ReturnValue(ValueModel value, bool shouldCancel)
            {
                Value = value;
                ShouldCancel = shouldCancel;
            }

            public ValueModel Value { get; }
            public bool ShouldCancel { get; }
        }

        public SelectEditor(IGameFactory factory, ActionManager actions, StateModel model, 
                            Func<IProperty, List<IStringItem>> getOptions, Func<IStringItem, IProperty, Action, Task<ReturnValue>> getValue)
        {
            _factory = factory;
            _actions = actions;
            _model = model;
            _getOptions = getOptions;
            _getValue = getValue;
        }

        public static IComboBox GetCombobox(string id, IGameFactory factory, IObject parent)
        {
            var dropDownButton = getDropDownButton(id, factory);
            var combobox = factory.UI.GetComboBox(id, dropDownButton, null, null, parent,
                               defaultWidth: 100f, defaultHeight: 25f, dropDownPanelOffset: 10f);
            combobox.TextBox.CaretXOffset = -15;

            var layout = combobox.DropDownPanel.ContentsPanel.GetComponent<IStackLayoutComponent>();
            layout.StopLayout();
            layout.RelativeSpacing = 0f;
            layout.AbsoluteSpacing = -25f;
            layout.StartLayout();

            GameViewColors.AddHoverEffect(combobox.TextBox, GameViewColors.ComboboxTextBorder, 
                                          GameViewColors.HoveredComboboxTextBorder, GameViewColors.ComboboxTextConfig, GameViewColors.ComboboxHoverTextConfig);
            combobox.DropDownPanel.ScrollingPanel.Tint = GameViewColors.TextEditor.WithAlpha(240);
            combobox.DropDownPanel.ScrollingPanel.Border = GameViewColors.DropDownBorder;

            return combobox;
        }

        public void AddEditorUI(string id, ITreeNodeView view, IProperty property)
        {
            _property = property;
            var label = view.TreeItem;
            var combobox = GetCombobox(id, _factory, label.TreeNode.Parent);
            _text = combobox.TextBox;

            var list = _getOptions(property);
            combobox.DropDownPanelList.Items.AddRange(list);
            combobox.Z = label.Z;
            combobox.TextBox.Text = property.ValueString.Humanize();
            if (list.Count > 5) //If more than 5 items in the dropdown, let's have it with textbox suggestions as user might prefer to type for filtering the dropdown
            {
                combobox.SuggestMode = ComboSuggest.Enforce;
            }
            var layout = view.HorizontalPanel.GetComponent<ITreeTableRowLayoutComponent>();
            if (layout != null)
            {
                layout.RestrictionList.RestrictionList.AddRange(new List<string> { combobox.ID });
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

        private static IButton getDropDownButton(string id, IGameFactory factory)
        {
            var textConfig = FontIcons.ButtonConfig;

            var idle = new ButtonAnimation(GameViewColors.ComboboxButtonBorder, textConfig, GameViewColors.TextEditor);
            var hover = new ButtonAnimation(GameViewColors.HoveredComboboxButtonBorder, null, GameViewColors.HoveredTextEditor);
            var pushed = new ButtonAnimation(GameViewColors.ComboboxButtonBorder, null, GameViewColors.PushedTextEditor);
            var dropDownButton = factory.UI.GetButton($"{id}_DropDownButton", idle, hover, pushed, 0f, 0f, null, FontIcons.CaretDown, textConfig, false, 25f, 25f);
            return dropDownButton;
        }
    }
}