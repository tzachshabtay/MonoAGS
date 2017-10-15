using System;
using AGS.API;

namespace AGS.Engine
{
    public class NumberPropertyEditor : IInspectorPropertyEditor
    {
        private readonly IUIFactory _factory;
        private readonly bool _wholeNumbers;

        public NumberPropertyEditor(IUIFactory factory, bool wholeNumbers)
        {
            _factory = factory;
            _wholeNumbers = wholeNumbers;
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
            var numberEditor = textbox.AddComponent<INumberEditorComponent>();
            numberEditor.EditWholeNumbersOnly = _wholeNumbers;
            numberEditor.Value = float.Parse(property.Value);
            numberEditor.OnValueChanged.Subscribe(() => 
            {
                if (_wholeNumbers)
                {
                    property.Prop.SetValue(property.Object, (int)Math.Round(numberEditor.Value));
                }
                else property.Prop.SetValue(property.Object, numberEditor.Value);
            });
        }
    }
}
