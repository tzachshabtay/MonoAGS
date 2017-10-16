using System;
using AGS.API;

namespace AGS.Engine
{
    public class NumberPropertyEditor : IInspectorPropertyEditor
    {
        private readonly IUIFactory _factory;
        private readonly IIconFactory _icons;
        private readonly bool _wholeNumbers;

        public NumberPropertyEditor(IUIFactory factory, IIconFactory icons, bool wholeNumbers)
        {
            _factory = factory;
            _icons = icons;
            _wholeNumbers = wholeNumbers;
        }

        public void AddEditorUI(string id, ITreeNodeView view, InspectorProperty property)
        {
            var label = view.TreeItem;
            const float rowHeight = 20f;
            AGSTextConfig config = new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText);
            var textbox = _factory.GetTextBox(id + "_Textbox",
                                              label.X, label.Y, label.TreeNode.Parent,
                                              property.Value, config, width: 100f, height: rowHeight);
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
            addArrowButtons(id, label, rowHeight, numberEditor);
        }

        private void addArrowButtons(string id, IUIControl label, float rowHeight, 
                                     INumberEditorComponent numberEditor)
        {
            var arrowUpIdle = _icons.GetArrowIcon(ArrowDirection.Up, Colors.White);
            var arrowDownIdle = _icons.GetArrowIcon(ArrowDirection.Down, Colors.White);
            var arrowUpHovered = _icons.GetArrowIcon(ArrowDirection.Up, Colors.Black);
            var arrowDownHovered = _icons.GetArrowIcon(ArrowDirection.Down, Colors.Black);
            var buttonsPanel = _factory.GetPanel(id + "_ButtonsPanel", 1f, 1f, 0f, label.Y, label.TreeNode.Parent);
            buttonsPanel.RenderLayer = label.RenderLayer;
            buttonsPanel.Tint = Colors.Transparent;
            float halfRowHeight = rowHeight / 2f;
            float buttonBottomPadding = 4.5f;
            float betweenButtonsPadding = 1f;
            float buttonHeight = halfRowHeight - betweenButtonsPadding * 2;
            var upButton = _factory.GetButton(id + "_UpButton", new ButtonAnimation(arrowUpIdle, null, Colors.Purple),
                                              new ButtonAnimation(arrowUpHovered, null, Colors.Yellow), new ButtonAnimation(arrowUpIdle, null, Colors.Blue),
                                              0f, buttonBottomPadding + buttonHeight + betweenButtonsPadding, buttonsPanel, width: 20f, height: buttonHeight);
            upButton.RenderLayer = label.RenderLayer;
            upButton.Z = label.Z;

            var downButton = _factory.GetButton(id + "_DownButton", new ButtonAnimation(arrowDownIdle, null, Colors.Purple),
                                                new ButtonAnimation(arrowDownHovered, null, Colors.Yellow), new ButtonAnimation(arrowDownIdle, null, Colors.Blue),
                                                0f, buttonBottomPadding, buttonsPanel, width: 20f, height: buttonHeight);
            downButton.RenderLayer = label.RenderLayer;
            downButton.Z = label.Z;
            numberEditor.UpButton = upButton;
            numberEditor.DownButton = downButton;
        }
    }
}
