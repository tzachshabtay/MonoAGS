using System;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class StringPropertyEditor : IInspectorPropertyEditor
    {
        private readonly IGameFactory _factory;
        private InspectorProperty _property;
        private readonly bool _enabled;
        private ITextComponent _textbox;
        private readonly ActionManager _actions;

        public StringPropertyEditor(IGameFactory factory, bool enabled, ActionManager actions)
        {
            _factory = factory;
            _enabled = enabled;
            _actions = actions;
        }

        public void AddEditorUI(string id, ITreeNodeView view, InspectorProperty property)
        {
            _property = property;
            var label = view.TreeItem;
            var config = _enabled ? GameViewColors.TextConfig : GameViewColors.ReadonlyTextConfig;
            if (!_enabled) view.HorizontalPanel.GetComponent<ITreeTableRowLayoutComponent>().RestrictionList.RestrictionList.Add(id);
            var textbox = _factory.UI.GetTextBox(id,
                                                 label.X, label.Y, label.TreeNode.Parent,
                                                 "", config, width: 100f, height: 20f);
            textbox.Text = property.Value;
            textbox.TextBackgroundVisible = false;
            textbox.Enabled = _enabled;
            if (_enabled)
            {
                GameViewColors.AddHoverEffect(textbox);
            }
            _textbox = textbox;
            textbox.RenderLayer = label.RenderLayer;
            textbox.Z = label.Z;
            HoverEffect.Add(textbox, Colors.Transparent, Colors.DarkSlateBlue);
            textbox.OnPressingKey.Subscribe(args =>
            {
                if (args.PressedKey != Key.Enter) return;
                args.ShouldCancel = true;
                textbox.IsFocused = false;
                setString();
            });
            textbox.LostFocus.Subscribe(args =>
            {
                textbox.IsFocused = false;
                setString();
            });
        }

        public void RefreshUI()
        {
            if (_textbox == null) return;
            _textbox.Text = _property.Value;
        }

        private void setString()
        {
            if (_actions.ActionIsExecuting) return;
            _actions.RecordAction(new PropertyAction(_property, _textbox.Text));
        }
    }
}