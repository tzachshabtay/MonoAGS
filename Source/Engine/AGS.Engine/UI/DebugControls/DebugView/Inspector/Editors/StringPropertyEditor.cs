using System;
using AGS.API;

namespace AGS.Engine
{
    public class StringPropertyEditor : IInspectorPropertyEditor
    {
        private readonly IUIFactory _factory;

        public StringPropertyEditor(IUIFactory factory)
        {
            _factory = factory;
        }

        public void AddEditorUI(string id, ITreeNodeView view, InspectorProperty property)
        {
			var label = view.TreeItem;
            AGSTextConfig config = new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText);
			var textbox = _factory.GetTextBox(id,
												 label.X, label.Y, label.TreeNode.Parent,
												 property.Value, config, width: 100f, height: 20f);
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
    }
}
