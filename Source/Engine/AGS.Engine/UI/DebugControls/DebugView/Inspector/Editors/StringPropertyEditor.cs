using System;
using AGS.API;

namespace AGS.Engine
{
    public class StringPropertyEditor : IInspectorPropertyEditor
    {
        private readonly IUIFactory _factory;
        private readonly bool _enabled;
        private InspectorProperty _property;
        private ITextComponent _textbox;

        public StringPropertyEditor(IUIFactory factory, bool enabled)
        {
            _factory = factory;
            _enabled = enabled;
        }

        public void AddEditorUI(string id, ITreeNodeView view, InspectorProperty property)
        {
            _property = property;
			var label = view.TreeItem;
            AGSTextConfig config = new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText);
			var textbox = _factory.GetTextBox(id,
												 label.X, label.Y, label.TreeNode.Parent,
												 "", config, width: 100f, height: 20f);
            textbox.Text = property.Value;
            textbox.TextBackgroundVisible = false;
            textbox.Enabled = _enabled;
            _textbox = textbox;
			textbox.RenderLayer = label.RenderLayer;
			textbox.Z = label.Z;
            HoverEffect.Add(textbox, Colors.Transparent, Colors.DarkSlateBlue);
			textbox.OnPressingKey.Subscribe(args =>
			{
                if (args.PressedKey != Key.Enter) return;
                args.ShouldCancel = true;
                textbox.IsFocused = false;
                property.Prop.SetValue(property.Object, textbox.Text);
			});
        }

        public void RefreshUI()
        {
            if (_textbox == null) return;
            _textbox.Text = _property.Value;
        }
    }
}
