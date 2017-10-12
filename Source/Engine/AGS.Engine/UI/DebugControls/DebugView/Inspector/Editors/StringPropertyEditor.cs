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
			var textbox = _factory.GetTextBox(id,
												 label.X, label.Y, label.TreeNode.Parent,
												 property.Value, width: 100f, height: 20f);
			textbox.RenderLayer = label.RenderLayer;
			textbox.Z = label.Z;
			textbox.Tint = Colors.Transparent;
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
